using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    
    public Dictionary<Spline,List<GameObject>> instantiatedObjects = new Dictionary<Spline, List<GameObject>>();
    public bool resetBeforeInstantiate = true;

    

    private void OnEnable()
    {
        if (splineContainer != null && splineContainer.Splines.All(spline => spline != null))
        {
            Spline.Changed += OnSplineChanged;
        }
    }

    private void OnDisable()
    {
        if (splineContainer != null && splineContainer.Splines.All(spline => spline != null))
        {
            Spline.Changed -= OnSplineChanged;
        }
    }

    
    /*private void OnValidate()
    {
        if (splineContainer != null && splineContainer.Splines.All(spline => spline != null))
        {
            UpdatePrefabsPosition();
        }
    }*/

    private void OnSplineChanged(Spline spline, int knotIndex,SplineModification modification)
    {
        UpdatePrefabsPosition(spline);
    }


    public void InstantiateAlongSplinesByDistance()
    {
        if (resetBeforeInstantiate)
        {
            ClearInstantiatedObjects();
        }


        for(int splineIndex = 0; splineIndex < splineContainer.Splines.Count; splineIndex++)
        {
            float splineLength = splineContainer.CalculateLength(splineIndex);
            int numberOfObjects = Mathf.FloorToInt(splineLength / spacing);

            float currentDistance = 0f;
            for (int j = 0; j <= numberOfObjects; j++)
            {
                float percentage = currentDistance / splineLength;

                UnityEngine.Vector3 position = splineContainer.EvaluatePosition(splineIndex, percentage);
                UnityEngine.Vector3 nextPosition = splineContainer.EvaluatePosition(splineIndex, percentage + spacingDirectionOffset);
                UnityEngine.Vector3 direction = nextPosition - position;
                UnityEngine.Quaternion rotation = UnityEngine.Quaternion.LookRotation(direction);

                GameObject instantiatedObject = Instantiate(prefab, position, rotation, splineContainer.transform);

                if (!instantiatedObjects.ContainsKey(splineContainer[splineIndex]))
                {
                    instantiatedObjects[splineContainer[splineIndex]] = new List<GameObject>() { instantiatedObject };
                }
                else
                {
                    instantiatedObjects[splineContainer[splineIndex]].Add(instantiatedObject);
                }


                currentDistance += spacing;


                Undo.RegisterCreatedObjectUndo(instantiatedObject, "Instantiate Prefabs Along Spline");
            }
        }
    }


    public void ClearInstantiatedObjects()
    {
        foreach(List<GameObject> splineObjlist in instantiatedObjects.Values)
        {
            foreach(GameObject obj in splineObjlist)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
        }
        instantiatedObjects.Clear();


        Undo.RecordObject(this, "Clear Instantiated Objects");
    }

    // Example method to change the color of an instantiated prefab
    /*
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
    }*/


    private void UpdatePrefabsPosition(Spline spline)
    {
        int splineIndex = -1;
        for(int i = 0; i < splineContainer.Splines.Count; i++)
        {
            if(splineContainer.Splines[i] == spline)
            {
                splineIndex = i;
                break;
            }
        }


        if (instantiatedObjects == null || instantiatedObjects.Count == 0 || splineContainer == null || splineContainer[splineIndex] == null || splineIndex == -1)
        {
            return; 
        }

        
        float splineLength = splineContainer.CalculateLength(splineIndex);
        int numberOfObjects = Mathf.FloorToInt(splineLength / spacing);

        if (instantiatedObjects[spline].Count < numberOfObjects)
        {
            for(int i = 0; i < numberOfObjects; i++)
            {
                GameObject instantiatedObject = Instantiate(prefab,splineContainer.transform);
                instantiatedObjects[spline].Add(instantiatedObject);
            }
        }

        if(instantiatedObjects.Count > numberOfObjects)
        {
            for(int i =0; i < instantiatedObjects[spline].Count - numberOfObjects; i++)
            {
                DestroyImmediate(instantiatedObjects[spline][i]);
            }
            instantiatedObjects[spline].RemoveRange(0, instantiatedObjects[spline].Count-numberOfObjects);
        }

        
        float currentDistance = 0f;
        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            float percentage = currentDistance / splineLength;

            UnityEngine.Vector3 position = splineContainer.EvaluatePosition(splineIndex,percentage);
            UnityEngine.Vector3 nextPosition = splineContainer.EvaluatePosition(splineIndex,percentage + spacingDirectionOffset);
            UnityEngine.Vector3 direction = nextPosition - position;
            UnityEngine.Quaternion rotation = UnityEngine.Quaternion.LookRotation(direction);

            
            instantiatedObjects[spline][i].transform.position = position;
            instantiatedObjects[spline][i].transform.rotation = rotation;


            currentDistance += spacing;
        }
    }
}