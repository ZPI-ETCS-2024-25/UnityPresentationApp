using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(RailDecorator))]
public class CustomSplineInstantiateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Reference to the script
        RailDecorator splineInstantiate = (RailDecorator)target;

        // Add a button to instantiate objects along the spline
        if (GUILayout.Button("Instantiate Along Spline"))
        {
            splineInstantiate.InstantiateAlongSplineByDistance();
        }

        // Add a button to clear instantiated objects
        if (GUILayout.Button("Clear Instantiated Objects"))
        {
            splineInstantiate.ClearInstantiatedObjects();
        }
    }
}
