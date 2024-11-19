using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class ObjectRenderDistance : MonoBehaviour
{
    public SplineDecorationObjects decorativeObjects;
    public SemaphoreManager semaphoreManager;
    public CrossingManager crossingManager;
    public float nearestSplineDistance;
    public float nearestObjectDistance;

    void Update()
    {
        UpdateRailDecorations();
        UpdateSemaphores();
        UpdateCrossings();
    }

    private void UpdateRailDecorations()
    {
        foreach (SplineAndDecorations splineAndDecorations in decorativeObjects.instantiatedObjects)
        {
            Spline spline = splineAndDecorations.spline;
            //Debug.Log("pre");
            //float distance = SplineUtility.GetNearestPoint(spline, gameObject.transform.position, out float3 closestPoint, out float t);
            //Debug.Log($"{spline}: {distance} limit {nearestSplineDistance}");
            if (SplineUtility.GetNearestPoint(spline, gameObject.transform.position, out float3 closestPoint, out float t) > nearestSplineDistance)
            {
                //Debug.Log($"setting false for {spline}");
                foreach (GameObject o in splineAndDecorations.decorations)
                {
                    o.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject o in splineAndDecorations.decorations)
                {
                    if (Vector3.Distance(gameObject.transform.position, o.transform.position) > nearestObjectDistance)
                    {
                        //Debug.Log("active single off");
                        o.SetActive(false);
                    }
                    else
                    {
                        o.SetActive(true);
                    }
                }
            }
        }
    }

    private void UpdateSemaphores()
    {
        foreach(SemaphoreController semaphore in semaphoreManager.semaphores)
        {
            if (Vector3.Distance(gameObject.transform.position, semaphore.transform.position) > nearestObjectDistance)
            {
                semaphore.SetActive(false);
            }
            else
            {
                semaphore.SetActive(true);
            }
        }
    }

    private void UpdateCrossings()
    {
        foreach (CrossingSystem crossing in crossingManager.crossings)
        {
            if (Vector3.Distance(gameObject.transform.position, crossing.transform.position) > nearestObjectDistance)
            {
                crossing.SetActive(false);
            }
            else
            {
                crossing.SetActive(true);
            }
        }
    }
}
