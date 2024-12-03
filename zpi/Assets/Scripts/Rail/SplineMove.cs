using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System;
using System.Numerics;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SplineMove : MonoBehaviour
{
    public UnityETCSComm comm;
    public Server server;
    public PathManager pathManager;
    public BaliseController baliseController;
    public LenghtManager lenghtManager;

    public bool checkForBalises = true;
    public bool sendSpeed = true;


    public SplineContainer splineContainer;
    public int startingSpline;
    private int currentSpline;
    private List<(int Spline,bool Backward)> previousSplineInfo;
    private int nextSplineIndex;

    [SerializeField]
    private GameObject[] wagons;
    private int[] wagonSplines;
    private bool[] wagonBackward;
    private float[] wagonDistance;


    public float startingSpeed = 1f;
    public float acceleration = 0.2f;
    public float maxAcceleration = 1f;
    public float minAcceleration = -1f;
    private bool endOfline = false;
    private bool forceBreak = false;


    public bool backwards = false;
    public float rotationCalculation = 0.03f;
    public float minRotationCalculation = 3f;
    private float speed = 0f;


    private float distancePercentage = 0f;
    private float previousDistancePercentage = 0f;


    public float wagonSeparationDistance = 0.03f;
    public float heightoffSet = 1f;


    void Start()
    {
        currentSpline = startingSpline;
        previousSplineInfo = new List<(int,bool)>() { (currentSpline,backwards) };
        nextSplineIndex = pathManager.GetNextSplineIndex(currentSpline,backwards);
        //nextSplinesInfo = new List<(UnityEngine.Vector3 KnotPosition, int PathIndex)>();
        speed = startingSpeed;

        if(server != null)
        {
            server.BreakCommand += StopTrainApp;
        }

        pathManager.JunctionChanged += UpdateNextSplineIndex;

        InitialiseWagons();
        //PrepareNextSplinesInfo();
        //PointArrow();
        
    }

    void FixedUpdate()
    {
        if (endOfline && comm != null)//jesli to koniec
        {          
            comm.SendSpeedInfo(0f);
            return;
        }

        previousDistancePercentage = distancePercentage;

        if (acceleration < minAcceleration)
        {
            acceleration = minAcceleration;
        }
        if (forceBreak)
        {
            acceleration = minAcceleration;
        }

        speed = Math.Max(speed + acceleration,0f);
        float speedKPH = speed * lenghtManager.modifier*50f*3600f;

        if (sendSpeed && comm != null)
        {
            comm.SendSpeedInfo(speedKPH);
        }

        
        MoveTrain(currentSpline, distancePercentage,out currentSpline,out distancePercentage);
        RotateObject(gameObject, currentSpline, distancePercentage, backwards);


        if (!previousSplineInfo.Any(info => info.Spline == currentSpline))
        {
            previousSplineInfo.Add((currentSpline,backwards));  
        }

        MoveWagons();


        if (checkForBalises)
        {
            if (baliseController != null)
            {
                List<BaliseInfo> baliseInformations = baliseController.checkForBalises(currentSpline, previousDistancePercentage, distancePercentage, backwards);
                if (baliseInformations != null && comm != null)
                {
                    foreach (BaliseInfo baliseInfo in baliseInformations)
                    {
                        comm.SendBaliseInfo(baliseInfo);
                    }
                }
            }
            else
            {
                Debug.Log("No ballise controler");
            }
        }
    }

    private void StopTrainApp(object o, bool stop)
    {
        if (stop)
        {
            forceBreak = true;
        }
        else
        {
            forceBreak=false;
        }
    }


    private void RotateObject(GameObject rotatingObject, int spline, float distancePercentage,bool movingBackward)
    {
        float rotationDistanceCalculation;
        if (!movingBackward)
        {
            rotationDistanceCalculation = Mathf.Min(1f, distancePercentage + rotationCalculation);
        }
        else
        {
            rotationDistanceCalculation = Mathf.Max(0f, distancePercentage - rotationCalculation);
        }

        UnityEngine.Vector3 position = rotatingObject.transform.position;
        UnityEngine.Vector3 nextPosition = splineContainer.EvaluatePosition(spline, rotationDistanceCalculation) + new Unity.Mathematics.float3(0, heightoffSet, 0);
        UnityEngine.Vector3 direction = nextPosition - position;

        if (direction != UnityEngine.Vector3.zero)
        {
            rotatingObject.transform.rotation = UnityEngine.Quaternion.LookRotation(direction, transform.up);
        }
    }


    private void MoveTrain(int spline, float currenctDistance, out int afterMovingSpline, out float afterMovingDistance)
    {
        float splineLength = splineContainer.CalculateLength(spline);
        float percentageTravelled = speed * Time.fixedDeltaTime / splineLength;

        float nextProcentage = backwards ? currenctDistance - percentageTravelled : currenctDistance + percentageTravelled;
        bool limitNotReached = backwards ? nextProcentage >= 0f                   : nextProcentage <= 1f;

        
        if (limitNotReached)
        {
            changeTrainPosition(spline, nextProcentage, out afterMovingSpline, out afterMovingDistance); //if limit not reached move train
        }
        else
        {
            try
            {
                Dictionary<int,List<(int Spline,bool Backward)>> path = backwards ? splineContainer.GetComponent<PathManager>().reversePath : splineContainer.GetComponent<PathManager>().path;

                if(!path.ContainsKey(spline)) { //stop if end of track
                    endOfline = true;
                    speed = 0f;
                    acceleration = 0f;
                    afterMovingSpline = spline;
                    afterMovingDistance = backwards ? 0f : 1f;
                    return;
                }

                int newSpline = path[spline][nextSplineIndex].Spline; //if reached change spline
                backwards = path[spline][nextSplineIndex].Backward; // and possibly direction of travel

                float percentageLeftover = backwards ? percentageTravelled - currenctDistance : currenctDistance + percentageTravelled - 1f; //how much procent was overfloved
                float newSplineLenght = splineContainer.CalculateLength(newSpline);
                float percentageLeftoverNewSpline = percentageLeftover * splineLength / newSplineLenght; //changing prom procent of old spline to procent of new spline
                percentageLeftoverNewSpline = backwards ? 1f - percentageLeftoverNewSpline : percentageLeftoverNewSpline; // if backwards then start coutning from 1f

                rotationCalculation = Math.Min(rotationCalculation * newSplineLenght, minRotationCalculation)/newSplineLenght;

                changeTrainPosition(newSpline, percentageLeftoverNewSpline, out afterMovingSpline, out afterMovingDistance);

                nextSplineIndex = pathManager.GetNextSplineIndex(currentSpline, backwards);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                float maximumPercentage = backwards ? 0f : 1f; //if something goes wrong stop at maximum/minimum spline percentage
                changeTrainPosition(spline, maximumPercentage, out afterMovingSpline, out afterMovingDistance);
            }
        }
    }


    private void changeTrainPosition(int spline, float percentage, out int splineOut, out float percentageOut)
    {
        gameObject.transform.position = splineContainer.EvaluatePosition(spline, percentage) + new Unity.Mathematics.float3(0, heightoffSet, 0);

        percentageOut = percentage;
        splineOut = spline;
    }


    private void MoveWagons()
    {
        for(int i = 0; i < wagons.Length; i++)
        {
            float splineLength = splineContainer.CalculateLength(wagonSplines[i]);
            float percentageTravelled = speed * Time.fixedDeltaTime / splineLength;

            float nextProcentage = backwards ? wagonDistance[i] - percentageTravelled : wagonDistance[i] + percentageTravelled;
            bool limitNotReached = backwards ? nextProcentage >= 0f : nextProcentage <= 1f;


            if (limitNotReached)
            {
                changeWagonPosition(i, nextProcentage);
            }
            else
            {
                try
                {
                    int currentSpline = previousSplineInfo.FindIndex(info => info.Spline == wagonSplines[i]);
                    int newSpline = previousSplineInfo[currentSpline + 1].Spline;
                    bool newDirection = previousSplineInfo[currentSpline + 1].Backward;
                    if (i == 0)
                    {
                        previousSplineInfo.RemoveAt(0);
                    }


                    float percentageLeftover = wagonBackward[i] ? percentageTravelled - wagonDistance[i] : wagonDistance[i] + percentageTravelled - 1f; //how much procent was overfloved
                    float newSplineLenght = splineContainer.CalculateLength(newSpline);
                    float percentageLeftoverNewSpline = percentageLeftover * splineLength / newSplineLenght; //changing prom procent of old spline to procent of new spline

                    wagonSplines[i] = newSpline;
                    wagonBackward[i] = newDirection;
                    wagonDistance[i] = wagonBackward[i] ? 1f - percentageLeftoverNewSpline : percentageLeftoverNewSpline;

                    changeWagonPosition(i, wagonDistance[i]);
                }
                catch(Exception e)
                {
                    Debug.Log(e.ToString());
                    float maximumPercentage = backwards ? 0f : 1f; //if something goes wrong stop at maximum/minimum spline percentage
                    changeWagonPosition(i, maximumPercentage);
                }
            }
        }
    }


    private void changeWagonPosition(int wagonIndex,float percentage)
    {
        wagons[wagonIndex].transform.position = splineContainer.EvaluatePosition(wagonSplines[wagonIndex], percentage) + new Unity.Mathematics.float3(0, heightoffSet, 0);
        wagonDistance[wagonIndex] = percentage;
        RotateObject(wagons[wagonIndex], wagonSplines[wagonIndex], wagonDistance[wagonIndex], wagonBackward[wagonIndex]);
        wagons[wagonIndex].transform.Rotate(-90, 0, 0); //correct rotation
    }


    private void InitialiseWagons()
    {
        wagonSplines = new int[wagons.Length];
        for (int i = 0; i < wagonSplines.Length; i++)
        {
            wagonSplines[i] = startingSpline;
        }


        wagonDistance = new float[wagons.Length];
        wagonBackward = new bool[wagons.Length];

        InitialiseWagonsPlacement();
    }

    
    private void InitialiseWagonsPlacement()
    {
        float trainScaleMultiplier = transform.localScale.x;
        float splineLength = splineContainer.CalculateLength(currentSpline);


        if (backwards)
        {
            for (int i = 0; i < wagons.Length; i++)
            {
                wagonDistance[i] = 1f;
            }
        }


        float wagonsLenghtSum = 0.0f;
        for (int i = 0; i < wagons.Length; i++)
        {
            wagonBackward[i] = backwards ? true :false;

            //this model has its length as y
            float wagonsLength = wagons[i].GetComponent<BoxCollider>().size.y * trainScaleMultiplier;
            wagonsLenghtSum += wagonsLength;

            if (!backwards)
            {
                for (int j = wagons.Length - 1; j > i; j--)
                {
                    wagonDistance[j] = wagonDistance[j] + (wagonsLength / splineLength);
                }
            }
            else
            {
                for (int j = 0; j < i; j++)
                {
                    wagonDistance[j] = wagonDistance[j] - (wagonsLength / splineLength);
                }
            }
        }


        if(!backwards)
        {
            distancePercentage += wagonsLenghtSum / splineLength;
        }
        else
        {
            distancePercentage = 1f;
            previousDistancePercentage = 1f;

            distancePercentage -= wagonsLenghtSum / splineLength;
        }
    }

    /*private void PrepareNextSplinesInfo()
    {
        nextSplinesInfo.Clear();

        try
        {
            List<(int, bool)> nextSplines = backwards ? splineContainer.GetComponent<PathManager>().reversePath[currentSpline]
                                                      : splineContainer.GetComponent<PathManager>().path[currentSpline];
            
            int index = 0;
            foreach ((int Spline, bool Backward) nextSpline in nextSplines)
            {
                if (nextSpline.Backward != true)
                {
                    nextSplinesInfo.Add((splineContainer.Splines[nextSpline.Spline][1].Position, index));
                }
                else
                {
                    int count = splineContainer.Splines[nextSpline.Spline].Count;
                    nextSplinesInfo.Add((splineContainer.Splines[nextSpline.Spline][count - 2].Position, index));
                }
                index++;
            }


            nextSplineIndex = 0;
        }
        catch
        {
            nextSplineIndex = -1;
        }
    }*/

    /*private void PointArrow()
    {
        if(nextSplineIndex != -1)
        {
            if(arrow == null)
            {
                arrow = Instantiate(arrowPrefab);
            }

            if (!arrow.activeSelf)
            {
                arrow.SetActive(true);
            }
            
            UnityEngine.Vector3 localPosition;
            if (!backwards)
            {
                int count = splineContainer[currentSpline].Count;
                localPosition = splineContainer[currentSpline][count-1].Position;
            }
            else
            {
                localPosition = splineContainer[currentSpline][0].Position;
            }
            UnityEngine.Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);
            UnityEngine.Vector3 localPositionNext = nextSplinesInfo[nextSplineIndex].KnotPosition;
            UnityEngine.Vector3 worldPositionNext = splineContainer.transform.TransformPoint(localPositionNext);

            UnityEngine.Vector3 direction = worldPositionNext - worldPosition;
            arrow.transform.position = worldPosition;
            arrow.transform.rotation = UnityEngine.Quaternion.LookRotation(direction, transform.up);
            arrow.transform.Rotate(0, 90, 0); //correct rotation
        }
        else
        {
            arrow.SetActive(false);
        }
    }*/


    public void changeTrackLeft()
    {
        /*if (nextSplineIndex  != 0 && nextSplineIndex != -1)
        {
            nextSplineIndex -= 1;
        }
        PointArrow();*/
    }

    public void changeTrackRight()
    {
        /*if (nextSplineIndex != nextSplinesInfo.Count-1 && nextSplineIndex != -1)
        {
            nextSplineIndex += 1;
        }
        PointArrow();*/
    }

    public void changeAcceleration(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        if(value == 1)
        {
            acceleration = maxAcceleration;
        }
        else
        {
            if(speed != 0)
            {
                acceleration = minAcceleration;
            }
        }
    }


    public void changeAccelerationChangeRate(float value)
    {
        if(value >= 0)
        {
            acceleration = maxAcceleration * value / 5f;
        }
        else
        {
            acceleration = minAcceleration * value  * (-1f)/ 5f;
        }
    }


    public void UpdateNextSplineIndex()
    {
        //Debug.Log(nextSplineIndex);
        nextSplineIndex = pathManager.GetNextSplineIndex(currentSpline, backwards);
        //Debug.Log(nextSplineIndex);
    }
}