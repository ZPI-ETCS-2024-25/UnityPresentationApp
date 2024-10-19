using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[ExecuteAlways]
public class RailDecorator : MonoBehaviour
{
    
    public SplineContainer splineContainer;
    public GameObject prefab;

    
    public float spacing = 2f;
    public float spacingDirectionOffset = 0.01f;

    
    public List<GameObject> instantiatedObjects = new List<GameObject>();
    public bool resetBeforeInstantiate = true;

    

    private void OnEnable()
    {
        if (splineContainer != null && splineContainer.Spline != null)
        {
            Spline.Changed += OnSplineChanged;
        }
    }

    private void OnDisable()
    {
        if (splineContainer != null && splineContainer.Spline != null)
        {
            Spline.Changed -= OnSplineChanged;
        }
    }

    private void OnValidate()
    {
        if (splineContainer != null && splineContainer.Spline != null)
        {
            UpdatePrefabsPosition();
        }
    }

    private void OnSplineChanged(Spline spline, int knotIndex,SplineModification modification)
    {
        UpdatePrefabsPosition();
    }


    public void InstantiateAlongSplineByDistance()
    {
        if (resetBeforeInstantiate)
        {
            ClearInstantiatedObjects();
        }

        // Get the total length of the spline
        float splineLength = splineContainer.CalculateLength();

        // Calculate how many objects we can fit based on the distance
        int numberOfObjects = Mathf.FloorToInt(splineLength / spacing);

        // Instantiate prefabs along the spline at the calculated distance intervals
        float currentDistance = 0f;
        for (int i = 0; i <= numberOfObjects; i++)
        {
            // Get the t (normalized position along the spline, from 0 to 1) corresponding to the current distance
            float t = currentDistance/splineLength;

            // Evaluate position on the spline at t
            UnityEngine.Vector3 position = splineContainer.EvaluatePosition(t);
            UnityEngine.Vector3 nextPosition = splineContainer.EvaluatePosition(t + spacingDirectionOffset);
            UnityEngine.Vector3 direction = nextPosition - position;
            UnityEngine.Quaternion rotation = UnityEngine.Quaternion.LookRotation(direction); // You can adjust this if you need specific orientation

            // Instantiate prefab and add it to the list
            GameObject instantiatedObject = Instantiate(prefab,position, rotation,splineContainer.transform);
            instantiatedObjects.Add(instantiatedObject);

            // Mark instantiated object for undo, so it can be undone in the Editor
            Undo.RegisterCreatedObjectUndo(instantiatedObject, "Instantiate Prefabs Along Spline");

            // Increase the distance for the next object
            currentDistance += spacing;
        }
    }


    public void ClearInstantiatedObjects()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        instantiatedObjects.Clear();

        Undo.RecordObject(this, "Clear Instantiated Objects");
    }

    // Example method to change the color of an instantiated prefab
    public void ChangePrefabColor(int index, Color color)
    {
        if (index >= 0 && index < instantiatedObjects.Count)
        {
            GameObject prefabToModify = instantiatedObjects[index];
            Renderer renderer = prefabToModify.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
        else
        {
            Debug.LogWarning("Index out of range");
        }
    }


    private void UpdatePrefabsPosition()
    {
        //Debug.Log("update");
        // Make sure we have instantiated objects and a valid spline
        if (instantiatedObjects == null || instantiatedObjects.Count == 0 || splineContainer == null || splineContainer.Spline == null)
        {
            return; // No objects to update or no valid spline
        }

        // Get the total length of the spline
        float splineLength = splineContainer.CalculateLength();
        int numberOfObjects = Mathf.FloorToInt(splineLength / spacing);

        if(instantiatedObjects.Count < numberOfObjects)
        {
            for(int i = 0; i < numberOfObjects; i++)
            {
                GameObject instantiatedObject = Instantiate(prefab,splineContainer.transform);
                instantiatedObjects.Add(instantiatedObject);
            }
        }

        if(instantiatedObjects.Count > numberOfObjects)
        {
            for(int i =0; i < instantiatedObjects.Count - numberOfObjects; i++)
            {
                DestroyImmediate(instantiatedObjects[i]);
            }
            instantiatedObjects.RemoveRange(0, instantiatedObjects.Count-numberOfObjects);
        }

        // Calculate the number of objects and distance between them
        float currentDistance = 0f;
        //Debug.Log(instantiatedObjects.Count);
        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            //Debug.Log(i);
            // Get the t (normalized position along the spline, from 0 to 1) corresponding to the current distance
            float t = currentDistance / splineLength;

            // Evaluate position on the spline at t
            UnityEngine.Vector3 position = splineContainer.EvaluatePosition(t);
            UnityEngine.Vector3 nextPosition = splineContainer.EvaluatePosition(t + spacingDirectionOffset);
            UnityEngine.Vector3 direction = nextPosition - position;
            UnityEngine.Quaternion rotation = UnityEngine.Quaternion.LookRotation(direction); // You can adjust this if you need specific orientation

            // Instantiate prefab and add it to the list
            instantiatedObjects[i].transform.position = position;
            instantiatedObjects[i].transform.rotation = rotation;

            // Mark instantiated object for undo, so it can be undone in the Editor
            //Undo.RegisterCreatedObjectUndo(instantiatedObjects[i], "Instantiate Prefabs Along Spline");

            // Increase the distance for the next object
            currentDistance += spacing;
        }
    }
}