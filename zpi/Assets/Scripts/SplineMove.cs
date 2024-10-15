using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMove : MonoBehaviour
{
    public SplineContainer spline;
    private float speed = 0f;
    private float distancePercentage = 0f;
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