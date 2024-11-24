using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectRenderDistance))]
public class CutomObjectRenderDistance : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectRenderDistance objectRenderDistance = (ObjectRenderDistance)target;


        if (GUILayout.Button("Get active splines"))
        {
            objectRenderDistance.PrintActive();
        }
    }
}
