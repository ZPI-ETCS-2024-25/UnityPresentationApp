using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathManager))]
public class CustomPathManager : Editor
{
    public override void OnInspectorGUI()
    {
        
        DrawDefaultInspector();

        
        PathManager pathManager = (PathManager)target;

        
        if (GUILayout.Button("Update Path"))
        {
            pathManager.UpdatePath();
        }

        
    }
}
