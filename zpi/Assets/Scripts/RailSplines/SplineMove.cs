using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System;
using System.Numerics;

public class SplineMove : MonoBehaviour
{
    public UnityETCSComm comm;


    public SplineContainer splineContainer;
    public int startingSpline;
    private int currentSpline;
    private List<int> standingOnSplines;


    [SerializeField]
    private GameObject[] wagons;
    private int[] wagonSplines;
    private float[] wagonDistance;


    public float startingSpeed = 1f;
    public float acceleration = 0.2f;
    public float rotationCalculation = 0.05f;
    private float speed = 0f;


    private float distancePercentage = 0f;
    private float previousDistancePercentage = 0f;


    public float wagonSeparationDistance = 0.03f;
    public float heightoffSet = 1f; 


    void Start()
    {
        currentSpline = startingSpline;
        standingOnSplines = new List<int>() { currentSpline };
        speed = startingSpeed;

        InitialiseWagons();
    }

    /*private void MoveWagons(float distancePercentage)
    {
        for (int i = 0; i < wagons.Length; i++)
        {
            float wagonDistancePercentage = distancePercentage - ((wagons[i].GetComponent<BoxCollider>().size.y ) * (i+1) * transform.localScale.x - GetComponent<BoxCollider>().size.y * (0.5f - wagonSeparationDistance)) / splineContainer.CalculateLength(currentSpline);
            Vector3 wagonPosition = splineContainer.EvaluatePosition(currentSpline,wagonDistancePercentage);
            wagons[i].transform.position = wagonPosition;

            RotateObject(wagons[i], currentSpline, wagonDistancePercentage);
            wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
        }
    }*/

    void FixedUpdate()
    {
        previousDistancePercentage = distancePercentage;
        speed = speed + acceleration;

        
        MoveTrain(gameObject, currentSpline, speed, distancePercentage,out distancePercentage,out currentSpline);
        RotateObject(gameObject, currentSpline, distancePercentage);
        //MoveWagons(distancePercentage);

        if (!standingOnSplines.Contains(currentSpline))
        {
            standingOnSplines.Add(currentSpline);  
        }

        MoveWagons(speed);
        
        
        BaliseController baliseController = splineContainer.GetComponent<BaliseController>();
        if(baliseController != null)
        {
            List<BaliseInfo> baliseInformations = baliseController.checkForBalises(currentSpline,previousDistancePercentage, distancePercentage, MathF.Sign(speed));
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


    private void RotateObject(GameObject rotatingObject,int spline,float distancePercentage)
    {
        if(previousDistancePercentage != distancePercentage)
        {
            float rotationDistanceCalculation = Mathf.Min(1f, distancePercentage + rotationCalculation);
            UnityEngine.Vector3 nextPosition = splineContainer.EvaluatePosition(spline, rotationDistanceCalculation) + new Unity.Mathematics.float3(0, heightoffSet, 0);
            UnityEngine.Vector3 direction = nextPosition - rotatingObject.transform.position;
            if(direction != UnityEngine.Vector3.zero)
            {
                rotatingObject.transform.rotation = UnityEngine.Quaternion.LookRotation(direction, transform.up);
            }
        }
    }

    private void MoveTrain(GameObject gameObject,int spline,float speed, float currenctDistance, out float afterMovingDistance, out int afterMovingSpline)
    {
        float splineLength = splineContainer.CalculateLength(spline);
        float percentageTravelled = speed * Time.fixedDeltaTime / splineLength;

        if(currenctDistance + percentageTravelled <= 1f)
        {
            gameObject.transform.position = splineContainer.EvaluatePosition(spline, currenctDistance + percentageTravelled) + new Unity.Mathematics.float3(0,heightoffSet,0);

            afterMovingDistance = currenctDistance + percentageTravelled;
            afterMovingSpline = spline;
        }
        else
        {
            try
            {
                int newSpline = splineContainer.GetComponent<PathManager>().path[spline][0];
                float moveLeftover = currenctDistance + percentageTravelled - 1f;
                float newSplineLenght = splineContainer.CalculateLength(newSpline);
                float percentageTravelledNewSpline = moveLeftover * splineLength / newSplineLenght;

                gameObject.transform.position = splineContainer.EvaluatePosition(newSpline, percentageTravelledNewSpline) + new Unity.Mathematics.float3(0, heightoffSet, 0);

                afterMovingDistance = percentageTravelledNewSpline;
                afterMovingSpline = newSpline;
            }
            catch
            {
                gameObject.transform.position = splineContainer.EvaluatePosition(spline, 1f) + new Unity.Mathematics.float3(0, heightoffSet, 0);

                afterMovingDistance = 1f;
                afterMovingSpline = spline;
            }
            
        }
    }


    private void MoveWagons(float speed)
    {
        for(int i = 0; i < wagons.Length; i++)
        {
            float splineLength = splineContainer.CalculateLength(wagonSplines[i]);
            float percentageTravelled = speed * Time.fixedDeltaTime / splineLength;

            if (wagonDistance[i] + percentageTravelled <= 1f)
            {
                wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], wagonDistance[i] + percentageTravelled) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                wagonDistance[i] = wagonDistance[i] + percentageTravelled;
                RotateObject(wagons[i], wagonSplines[i], wagonDistance[i]);
                wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
            }
            else
            {
                try
                {
                    int currentSpline = standingOnSplines.IndexOf(wagonSplines[i]);
                    int newSpline = standingOnSplines[currentSpline + 1];
                    if (i == 0)
                    {
                        standingOnSplines.RemoveAt(0);
                    }
                    float moveLeftover = wagonDistance[i] + percentageTravelled - 1f;
                    float newSplineLenght = splineContainer.CalculateLength(newSpline);
                    float percentageTravelledNewSpline = moveLeftover * splineLength / newSplineLenght;

                    wagonSplines[i] = newSpline;
                    wagonDistance[i] = percentageTravelledNewSpline;

                    wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], wagonDistance[i]) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                    RotateObject(wagons[i], wagonSplines[i], wagonDistance[i]);
                    wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
                }
                catch
                {
                    wagons[i].transform.position = splineContainer.EvaluatePosition(wagonSplines[i], 1f) + new Unity.Mathematics.float3(0, heightoffSet, 0);
                    wagonDistance[i] = 1f;
                    RotateObject(wagons[i], wagonSplines[i], wagonDistance[i]);
                    wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
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


        InitialiseWagonsPlacement();
    }


    private void InitialiseWagonsPlacement()
    {
        float trainScaleMultiplier = transform.localScale.x;
        float splineLength = splineContainer.CalculateLength(currentSpline);

        float wagonsLenghtSum = 0.0f;
        for (int i = 0; i < wagons.Length; i++)
        {
            //this model has its length as y
            float wagonsLength = wagons[i].GetComponent<BoxCollider>().size.y * trainScaleMultiplier;
            wagonsLenghtSum += wagonsLength;

            for (int j = wagons.Length-1; j > i; j--)
            {
                wagonDistance[j] = wagonDistance[j] + (wagonsLength / splineLength);
            }
        }
        distancePercentage += wagonsLenghtSum / splineLength;
    }
}