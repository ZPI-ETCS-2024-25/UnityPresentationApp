using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMove : MonoBehaviour
{
    public UnityETCSComm comm;
    public SplineContainer spline;
    private float speed = 0f;
    private float distancePercentage = 0f;
    private int lastBalise = 0;
    public float startingSpeed = 1f;
    public float acceleration = 0.2f;

    float splineLength;

    void Start()
    {
        splineLength = spline.CalculateLength();
        speed = startingSpeed;
    }

    void FixedUpdate()
    {
        distancePercentage += speed * Time.deltaTime / splineLength;
        speed = speed + acceleration;

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;


        BaliseController baliseController = spline.GetComponent<BaliseController>();
        if(baliseController != null)
        {
            BaliseInfo baliseInfo = baliseController.checkForBalises(lastBalise, distancePercentage, speed > 0);
            if (baliseInfo != null)
            {
                lastBalise = baliseInfo.number;
                Debug.Log($"Balise info: {baliseInfo.number}/{baliseInfo.groupSize},{baliseInfo.kilometer},{baliseInfo.track}");
                comm.SendBaliseInfo(baliseInfo);
            }
        }
        else
        {
            Debug.Log("No ballise controler");
        }
        

        if (distancePercentage > 1f)
        {
            distancePercentage = 1f;
        }
        if(distancePercentage < 0f)
        {
            distancePercentage = 0f;
        }

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}