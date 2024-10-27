using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System;

public class SplineMove : MonoBehaviour
{
    public UnityETCSComm comm;


    public SplineContainer splineContainer;
    public int startingSpline;
    private int currentSpline;


    [SerializeField]
    private GameObject[] wagons;


    public float startingSpeed = 1f;
    public float acceleration = 0.2f;
    public float rotationCalculation = 0.05f;
    private float speed = 0f;

    private float distancePercentage = 0f;
    private float previousDistancePercentage = 0f;


    public float wagonSeparationDistance = 0.03f;
    float splineLength;
    float trainScaleMultiplier;


    void Start()
    {
        currentSpline = startingSpline;
        speed = startingSpeed;


        trainScaleMultiplier = transform.localScale.x;
        splineLength = splineContainer.CalculateLength(currentSpline);

        float wagonsLength = 0.0f;
        for (int i = 0; i < wagons.Length; i++)
        {
            //this model has its length as y
            wagonsLength += wagons[i].GetComponent<BoxCollider>().size.y * trainScaleMultiplier;
        }

        distancePercentage += wagonsLength / splineLength;
    }

    private void MoveWagons(float distancePercentage)
    {
        for (int i = 0; i < wagons.Length; i++)
        {
            float wagonDistancePercentage = distancePercentage - ((wagons[i].GetComponent<BoxCollider>().size.y ) * (i+1) * trainScaleMultiplier - GetComponent<BoxCollider>().size.y * (0.5f - wagonSeparationDistance)) / splineLength;
            Vector3 wagonPosition = splineContainer.EvaluatePosition(currentSpline,wagonDistancePercentage);
            wagons[i].transform.position = wagonPosition;

            RotateObject(wagons[i], currentSpline, wagonDistancePercentage);
            wagons[i].transform.Rotate(-90, 0, 0); //correct rotation
        }
    }

    void FixedUpdate()
    {
        previousDistancePercentage = distancePercentage;
        speed = speed + acceleration;

        //distancePercentage += speed * Time.deltaTime / splineLength;

        /*
        if (distancePercentage > 1f)
        {
            distancePercentage = 1f;
        }
        if (distancePercentage < 0f)
        {
            distancePercentage = 0f;
        }*/

        //Vector3 currentPosition = splineContainer.EvaluatePosition(currentSpline, distancePercentage);
        //transform.position = currentPosition;
        MoveObject(gameObject, currentSpline, speed, distancePercentage,out distancePercentage,out currentSpline);
        RotateObject(gameObject, currentSpline, distancePercentage);
        //MoveWagons(distancePercentage);




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
            Vector3 nextPosition = splineContainer.EvaluatePosition(spline, rotationDistanceCalculation);
            Vector3 direction = nextPosition - rotatingObject.transform.position;
            if(direction != Vector3.zero)
            {
                rotatingObject.transform.rotation = Quaternion.LookRotation(direction, transform.up);
            }
        }
    }

    private void MoveObject(GameObject gameObject,int spline,float speed, float currenctDistance, out float afterMovingDistance, out int afterMovingSpline)
    {
        float splineLength = splineContainer.CalculateLength(spline);
        float percentageTravelled = speed * Time.fixedDeltaTime / splineLength;

        if(currenctDistance + percentageTravelled <= 1f)
        {
            gameObject.transform.position = splineContainer.EvaluatePosition(spline, currenctDistance + percentageTravelled);

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

                gameObject.transform.position = splineContainer.EvaluatePosition(newSpline, percentageTravelledNewSpline);

                afterMovingDistance = percentageTravelledNewSpline;
                afterMovingSpline = newSpline;
            }
            catch
            {
                gameObject.transform.position = splineContainer.EvaluatePosition(spline, 1f);

                afterMovingDistance = 1f;
                afterMovingSpline = spline;
            }
            
        }
    }
}