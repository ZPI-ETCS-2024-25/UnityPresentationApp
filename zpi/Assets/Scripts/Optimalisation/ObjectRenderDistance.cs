using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Threading;

public class ObjectRenderDistance : MonoBehaviour
{
    public SplineContainer splineContainer;
    public SplineDecorationObjects decorativeObjects;
    public SemaphoreManager semaphoreManager;
    public CrossingManager crossingManager;
    public float nearestSplineDistance;
    public float nearestObjectDistance;
    private bool[] activeSplines;
    private Vector3[] splineCenters;
    private float[] splineLenghts;
    public int debugSpline;

    public void Start()
    {
        activeSplines = new bool[splineContainer.Splines.Count];
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            activeSplines[i] = true;
        }
        splineCenters = new Vector3[splineContainer.Splines.Count];
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            float3 center = Vector3.zero;
            for(int j = 0; j < splineContainer.Splines[i].Count; j++)
            {
                center += splineContainer[i][j].Position;
            }
            splineCenters[i] = center / splineContainer.Splines[i].Count;
        }
        splineLenghts = new float[splineContainer.Splines.Count];
        for(int i = 0; i<splineContainer.Splines.Count; i++)
        {
            splineLenghts[i] = splineContainer.CalculateLength(i);
        }
    }

    void Update()
    {
        if(decorativeObjects != null)
        {
            UpdateRailDecorations();
        }
        if(semaphoreManager != null)
        {
            UpdateSemaphores();
        }
        if(crossingManager != null)
        {
            UpdateCrossings();
        }
    }

    private void UpdateRailDecorations()
    {
        int count = 0;
        foreach (SplineAndDecorations splineAndDecorations in decorativeObjects.instantiatedObjects)
        {
            Spline spline = splineAndDecorations.spline;
            Vector3 myDistnace = splineContainer.transform.InverseTransformPoint(gameObject.transform.position);
            float nearestDistance = SplineUtility.GetNearestPoint(spline, myDistnace,out float3 closestPoint, out float t,iterations:5);
            if(count == debugSpline)
            {
                //Debug.Log(nearestDistance);
                //Debug.Log(splineLenghts[count]);
                //Debug.Log(Vector3.Distance(gameObject.transform.position, splineCenters[count]));
                //Debug.Log($"{nearestDistance > nearestSplineDistance} {Vector3.Distance(gameObject.transform.position, splineCenters[count]) > splineLenghts[count]}");
                //Debug.Log(closestPoint);
            }
            if (nearestDistance > nearestSplineDistance && Vector3.Distance(myDistnace, splineCenters[count]) > splineLenghts[count]/1.6f)
            {
                if (activeSplines[count])
                {
                    activeSplines[count] = false;
                    foreach (GameObject o in splineAndDecorations.decorations)
                    {
                        if (o.activeSelf)
                        {
                            o.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                foreach (GameObject o in splineAndDecorations.decorations)
                {
                    if (Vector3.Distance(gameObject.transform.position, o.transform.position) > nearestObjectDistance)
                    {
                        if (o.activeSelf)
                        {
                            o.SetActive(false);
                        }
                    }
                    else
                    {
                        if (!activeSplines[count])
                        {
                            activeSplines[count] = true;
                        }
                        if (!o.activeSelf)
                        {
                            o.SetActive(true);
                        }
                    }
                }
            }
            count++;
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

    public void PrintActive()
    {
        string s = "";
        for (int i = 0; i < activeSplines.Length; i++)
        {
            if (activeSplines[i])
            {
                s += i.ToString() + " ";
            }
        }
        Debug.Log(s);
    }
}
