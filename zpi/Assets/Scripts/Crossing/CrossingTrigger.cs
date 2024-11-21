using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossingTrigger : MonoBehaviour
{
    [SerializeField] private GameObject crossing;
    private CrossingSystem crossingSystem;
    private float triggerDistanceFromCrossing;

    private void Start()
    {
        crossingSystem = crossing.GetComponent<CrossingSystem>();

        triggerDistanceFromCrossing = Vector3.Distance(this.transform.position, crossing.transform.position);
        //Debug.Log(this.name + ": " + triggerDistanceFromCrossing);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train"))
        {
            float trainDistanceFromCrossing = Vector3.Distance(other.transform.position, crossing.transform.position);
            //Debug.Log(other.name + ": " + trainDistanceFromCrossing);

            bool isMovingForward = trainDistanceFromCrossing > triggerDistanceFromCrossing;

            if (isMovingForward)
            {
                crossingSystem.CloseCrossing();
            }
            else
            {
                crossingSystem.OpenCrossing();
            }

            crossingSystem.SendCrossingStateToServer();
        }
    }
}
