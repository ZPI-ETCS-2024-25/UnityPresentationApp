using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System;
using System.Numerics;
using System.Linq;

public class SplineMove : MonoBehaviour
{
    public UnityETCSComm comm;


    public SplineContainer splineContainer;
    public int startingSpline;
    private int currentSpline;
    private List<(int Spline,bool Backward)> previousSplineInfo;

    private List<(UnityEngine.Vector3 KnotPosition, int PathIndex)> nextSplinesInfo;
    private int nextSplineIndex;
    public GameObject arrowPrefab;
    private GameObject arrow;

    [SerializeField]
    private GameObject[] wagons;
    private int[] wagonSplines;
    private bool[] wagonBackward;
    private float[] wagonDistance;


    public float startingSpeed = 1f;
    public float acceleration = 0.2f;
    public bool backwards = false;
    public float rotationCalculation = 0.05f;
    private float speed = 0f;


    private float distancePercentage = 0f;
    private float previousDistancePercentage = 0f;


    public float wagonSeparationDistance = 0.03f;
    public float heightoffSet = 1f; 


    void Start()
    {
        currentSpline = startingSpline;
        previousSplineInfo = new List<(int,bool)>() { (currentSpline,backwards) };
        nextSplinesInfo = new List<(UnityEngine.Vector3 KnotPosition, int PathIndex)>();
        speed = startingSpeed;

        InitialiseWagons();
        PrepareNextSplinesInfo();
        PointArrow();
    }

    void FixedUpdate()
    {
        previousDistancePercentage = distancePercentage;
        speed = speed + acceleration;

        
        MoveTrain(currentSpline, distancePercentage,out currentSpline,out distancePercentage);
        RotateObject(gameObject, currentSpline, distancePercentage, backwards);


        if (!previousSplineInfo.Any(info => info.Spline == currentSpline))
        {
            previousSplineInfo.Add((currentSpline,backwards));  
        }

        MoveWagons();
        
        
        BaliseController baliseController = splineContainer.GetComponent<BaliseController>();
        if(baliseController != null)
        {
            List<BaliseInfo> baliseInformations = baliseController.checkForBalises(currentSpline,previousDistancePercentage, distancePercentage, backwards);
            if (baliseInformations != null)
            {
                foreach(BaliseInfo baliseInfo in baliseInformations)
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

        if (!backwards)
        {
            if (currenctDistance + percentageTravelled <= 1f)
            {
                changeTrainPosition(spline, currenctDistance + percentageTravelled, out afterMovingSpline, out afterMovingDistance);
            }
            else
            {
                try
                {
                    int newSpline = splineContainer.GetComponent<PathManager>().path[spline][nextSplineIndex].Spline;
                    backwards = splineContainer.GetComponent<PathManager>().path[spline][nextSplineIndex].Backward;
                    float moveLeftover = currenctDistance + percentageTravelled - 1f;
                    float newSplineLenght = splineContainer.CalculateLength(newSpline);
                    float percentageTravelledNewSpline = moveLeftover * splineLength / newSplineLenght;

                    if (!backwards)
                    {
                        changeTrainPosition(newSpline, percentageTravelledNewSpline, out afterMovingSpline, out afterMovingDistance);
                    }
                    else
                    {
                        changeTrainPosition(newSpline, 1f-percentageTravelledNewSpline, out afterMovingSpline, out afterMovingDistance);
                    }
                    PrepareNextSplinesInfo();
                    PointArrow();
                }
                catch
                {
                    changeTrainPosition(spline, 1f, out afterMovingSpline, out afterMovingDistance);
                }
            }
        }
        else
        {
            if (currenctDistance - percentageTravelled >= 0f)
            {
                changeTrainPosition(spline, currenctDistance - percentageTravelled, out afterMovingSpline, out afterMovingDistance);
            }
            else
            {
                try
                {
                    int newSpline = splineContainer.GetComponent<PathManager>().reversePath[spline][nextSplineIndex].Spline;
                    backwards = splineContainer.GetComponent<PathManager>().reversePath[spline][nextSplineIndex].Backward;

                    float moveLeftover = percentageTravelled - currenctDistance;
                    float newSplineLenght = splineContainer.CalculateLength(newSpline);
                    float percentageTravelledNewSpline = moveLeftover * splineLength / newSplineLenght;
                    
                    if (!backwards)
                    {
                        changeTrainPosition(newSpline, percentageTravelledNewSpline, out afterMovingSpline, out afterMovingDistance); 
                    }
                    else
                    {
                        changeTrainPosition(newSpline, 1f - percentageTravelledNewSpline, out afterMovingSpline, out afterMovingDistance);
                    }
                    PrepareNextSplinesInfo();
                    PointArrow();
                }
                catch
                {
                    changeTrainPosition(spline, 0f, out afterMovingSpline, out afterMovingDistance);
                }
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


            if (!wagonBackward[i])
            {
                if (wagonDistance[i] + percentageTravelled <= 1f)
                {
                    wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], wagonDistance[i] + percentageTravelled) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                    wagonDistance[i] = wagonDistance[i] + percentageTravelled;
                    RotateObject(wagons[i], wagonSplines[i], wagonDistance[i], wagonBackward[i]);
                    wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
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
                        float moveLeftover = wagonDistance[i] + percentageTravelled - 1f;
                        float newSplineLenght = splineContainer.CalculateLength(newSpline);
                        float percentageTravelledNewSpline = moveLeftover * splineLength / newSplineLenght;

                        wagonSplines[i] = newSpline;
                        wagonBackward[i] = newDirection;
                        wagonDistance[i] = wagonBackward[i] ? 1f-percentageTravelled : percentageTravelledNewSpline;
                        
                        wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], wagonDistance[i]) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                        RotateObject(wagons[i], wagonSplines[i], wagonDistance[i], wagonBackward[i]);
                        wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
                    }
                    catch
                    {
                        wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], 1f) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                        wagonDistance[i] = 1f;
                        RotateObject(wagons[i], wagonSplines[i], wagonDistance[i], wagonBackward[i]);
                        wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
                    }
                }
            }
            else
            {
                if (wagonDistance[i] - percentageTravelled >= 0f)
                {
                    wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], wagonDistance[i] - percentageTravelled) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                    wagonDistance[i] = wagonDistance[i] - percentageTravelled;
                    RotateObject(wagons[i], wagonSplines[i], wagonDistance[i], wagonBackward[i]);
                    wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
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
                        float moveLeftover = percentageTravelled - wagonDistance[i];
                        float newSplineLenght = splineContainer.CalculateLength(newSpline);
                        float percentageTravelledNewSpline = moveLeftover * splineLength / newSplineLenght;

                        wagonSplines[i] = newSpline;
                        wagonBackward[i] = newDirection;
                        wagonDistance[i] = wagonBackward[i] ? 1f - percentageTravelled : percentageTravelledNewSpline;

                        wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], wagonDistance[i]) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                        RotateObject(wagons[i], wagonSplines[i], wagonDistance[i], wagonBackward[i]);
                        wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
                    }
                    catch
                    {
                        wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], 0f) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                        wagonDistance[i] = 0f;
                        RotateObject(wagons[i], wagonSplines[i], wagonDistance[i], wagonBackward[i]);
                        wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
                    }
                }
            }
        }
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

        if (!backwards)
        {
            InitialiseWagonsPlacement();
        }
        else
        {
            InitialiseWagonsPlacementReverse();
        }
    }


    private void InitialiseWagonsPlacement()
    {
        float trainScaleMultiplier = transform.localScale.x;
        float splineLength = splineContainer.CalculateLength(currentSpline);



        float wagonsLenghtSum = 0.0f;
        for (int i = 0; i < wagons.Length; i++)
        {
            wagonBackward[i] = false;



            //this model has its length as y
            float wagonsLength = wagons[i].GetComponent<BoxCollider>().size.y * trainScaleMultiplier;
            wagonsLenghtSum += wagonsLength;
   
            for (int j = wagons.Length - 1; j > i; j--)
            {
                wagonDistance[j] = wagonDistance[j] + (wagonsLength / splineLength);
            }
        }   
        distancePercentage += wagonsLenghtSum / splineLength;
    }



    private void InitialiseWagonsPlacementReverse()
    {
        float trainScaleMultiplier = transform.localScale.x;
        float splineLength = splineContainer.CalculateLength(currentSpline);


        for(int i = 0; i < wagons.Length; i++)
        {
            wagonDistance[i] = 1f;
        }

        float wagonsLenghtSum = 0.0f;
        for (int i = 0; i < wagons.Length; i++)
        {
            wagonBackward[i] = true;



            //this model has its length as y
            float wagonsLength = wagons[i].GetComponent<BoxCollider>().size.y * trainScaleMultiplier;
            wagonsLenghtSum += wagonsLength;
            
            for (int j = 0; j < i; j++)
            {
                wagonDistance[j] = wagonDistance[j] - (wagonsLength / splineLength);
            } 
        }
        distancePercentage = 1f;
        previousDistancePercentage = 1f;

        distancePercentage -= wagonsLenghtSum / splineLength;
        
    }


    private void PrepareNextSplinesInfo()
    {
        nextSplinesInfo.Clear();

        try
        {
            List<(int, bool)> nextSplines;
            if (!backwards)
            {
                nextSplines = splineContainer.GetComponent<PathManager>().path[currentSpline];
            }
            else
            {
                nextSplines = splineContainer.GetComponent<PathManager>().reversePath[currentSpline];
            }

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
    }

    private void PointArrow()
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
    }
}