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
    private int lastBalise = 0;
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
        

        

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}