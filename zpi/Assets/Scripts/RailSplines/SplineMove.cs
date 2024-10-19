using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMove : MonoBehaviour
{
    public UnityETCSComm comm;
    public SplineContainer spline;
    [SerializeField] private GameObject[] wagons;
    private float speed = 0f;
    private float distancePercentage = 0f;
    private float previousDistancePercentage = 0f;
    public float startingSpeed = 1f;
    public float acceleration = 0.2f;

    float splineLength;
    float trainScaleMultiplier;

    void Start()
    {
        splineLength = spline.CalculateLength();
        speed = startingSpeed;
        trainScaleMultiplier = transform.localScale.x;

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
            float wagonDistancePercentage = distancePercentage - ((wagons[i].GetComponent<BoxCollider>().size.y ) * (i+1) * trainScaleMultiplier - GetComponent<BoxCollider>().size.y * 0.47f) / splineLength;
            Vector3 wagonPosition = spline.EvaluatePosition(wagonDistancePercentage);
            wagons[i].transform.position = wagonPosition;

            Vector3 nextPosition = spline.EvaluatePosition(wagonDistancePercentage + 0.05f);
            Vector3 direction = nextPosition - wagonPosition ;
            wagons[i].transform.rotation = Quaternion.LookRotation(direction, transform.up);
            //fix rotation of wagon
            wagons[i].transform.Rotate(-90, 0, 0);
        }
    }

    void FixedUpdate()
    {
        previousDistancePercentage = distancePercentage;
        distancePercentage += speed * Time.deltaTime / splineLength;
        speed = speed + acceleration;
        
        if (distancePercentage > 1f)
        {
            distancePercentage = 1f;
        }
        if (distancePercentage < 0f)
        {
            distancePercentage = 0f;
        }

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        MoveWagons(distancePercentage);

        BaliseController baliseController = spline.GetComponent<BaliseController>();
        if(baliseController != null)
        {
            int speedDirection;
            if(speed == 0f)
            {
                speedDirection = 0;
            }
            else if(speed > 0f)
            {
                speedDirection = 1;
            }
            else
            {
                speedDirection = -1;
            }

            //Debug.Log($"{previousDistancePercentage} {distancePercentage}");
            List<BaliseInfo> baliseInformations = baliseController.checkForBalises(previousDistancePercentage, distancePercentage, speedDirection);
            if (baliseInformations != null)
            {
                foreach(BaliseInfo baliseInfo in baliseInformations)
                {
                    //Debug.Log($"sending {baliseInfo}");
                    comm.SendBaliseInfo(baliseInfo);
                }
            }
        }
        else
        {
            Debug.Log("No ballise controler");
        }
        

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}