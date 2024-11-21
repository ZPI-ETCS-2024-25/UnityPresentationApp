using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class RailDecorator : MonoBehaviour
{
    
    public SplineContainer splineContainer;
    public GameObject prefab;

    
    public float spacing = 2f;
    public float spacingDirectionOffset = 0.01f;

    [SerializeField]
    public SplineDecorationObjects splineDecorativeObjects;
    public bool resetBeforeInstantiate = true;


    public void Start()
    {
        ClearInstantiatedObjects();
        InstantiateAlongSplinesByDistance();
    }


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

        

        for (int splineIndex = 0; splineIndex < splineContainer.Splines.Count; splineIndex++)
        {
            float splineLength = splineContainer.CalculateLength(splineIndex);
            int numberOfObjects = Mathf.FloorToInt(splineLength / spacing);

            float currentDistance = 0f;
            for (int j = 0; j <= numberOfObjects; j++)
            {
                float percentage = currentDistance / splineLength;

                Vector3 position = splineContainer.EvaluatePosition(splineIndex, percentage);
                Vector3 nextPosition = splineContainer.EvaluatePosition(splineIndex, percentage + spacingDirectionOffset);
                Vector3 direction = nextPosition - position;
                Quaternion rotation = Quaternion.LookRotation(direction);

                GameObject instantiatedObject = Instantiate(prefab, position, rotation, splineContainer.transform);

                if (!splineDecorativeObjects.instantiatedObjects.Any(pair => pair.spline == splineContainer[splineIndex]))
                {
                    splineDecorativeObjects.instantiatedObjects.Add(new SplineAndDecorations() { 
                                                                                                spline = splineContainer[splineIndex],
                                                                                                decorations = new List<GameObject>() {instantiatedObject } 
                                                                                                });
                }
                else
                {
                    splineDecorativeObjects.instantiatedObjects.Find(pair => pair.spline == splineContainer[splineIndex]).decorations.Add(instantiatedObject);
                }


                currentDistance += spacing;


                Undo.RegisterCreatedObjectUndo(instantiatedObject, "Instantiate Prefabs Along Spline");
            }
        }
    }


    public void ClearInstantiatedObjects()
    {
        for (int i = 0; i < splineDecorativeObjects.instantiatedObjects.Count; i++)
        {
            foreach(GameObject obj in splineDecorativeObjects.instantiatedObjects[i].decorations)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
        }
        splineDecorativeObjects.instantiatedObjects.Clear();


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


        if (splineDecorativeObjects == null || splineDecorativeObjects.instantiatedObjects.Count == 0 || splineContainer == null || splineContainer[splineIndex] == null || splineIndex == -1)
        {
            return; 
        }

        
        float splineLength = splineContainer.CalculateLength(splineIndex);
        int numberOfObjects = Mathf.FloorToInt(splineLength / spacing);


        SplineAndDecorations splineAndDecorations =  splineDecorativeObjects.instantiatedObjects.Find(pair => pair.spline == splineContainer[splineIndex]);

        if (splineAndDecorations.decorations.Count < numberOfObjects)
        {
            for(int i = 0; i < numberOfObjects; i++)
            {
                GameObject instantiatedObject = Instantiate(prefab,splineContainer.transform);
                splineAndDecorations.decorations.Add(instantiatedObject);
            }
        }

        if(splineAndDecorations.decorations.Count > numberOfObjects)
        {
            for(int i =0; i < splineAndDecorations.decorations.Count - numberOfObjects; i++)
            {
                DestroyImmediate(splineAndDecorations.decorations[i]);
            }
            splineAndDecorations.decorations.RemoveRange(0, splineAndDecorations.decorations.Count-numberOfObjects);
        }

        
        float currentDistance = 0f;
        for (int i = 0; i < splineAndDecorations.decorations.Count; i++)
        {
            float percentage = currentDistance / splineLength;

            Vector3 position = splineContainer.EvaluatePosition(splineIndex,percentage);
            Vector3 nextPosition = splineContainer.EvaluatePosition(splineIndex,percentage + spacingDirectionOffset);
            Vector3 direction = nextPosition - position;
            Quaternion rotation = Quaternion.LookRotation(direction);

            
            splineAndDecorations.decorations[i].transform.position = position;
            splineAndDecorations.decorations[i].transform.rotation = rotation;


            currentDistance += spacing;
        }
    }
}