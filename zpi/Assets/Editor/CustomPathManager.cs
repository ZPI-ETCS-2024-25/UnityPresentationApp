using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathManager))]
public class CustomPathManager : Editor
{
    private bool showButtons = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PathManager pathManager = (PathManager)target;

        showButtons = EditorGUILayout.Foldout(showButtons, "Testing");

        if (showButtons)
        {
            if (GUILayout.Button("Show path"))
            {
                Debug.Log("Path forward");
                pathManager.DebugPrintPath(pathManager.path);
            }
            if (GUILayout.Button("Show reverse path"))
            {
                Debug.Log("Path backward");
                pathManager.DebugPrintPath(pathManager.reversePath);
            }
            if (GUILayout.Button("Show junctions"))
            {
                Debug.Log("Junctions forward");
                pathManager.DebugPrintJunctions(pathManager.junctions,pathManager.path);
            }
            if (GUILayout.Button("Show reverse junctions"))
            {
                Debug.Log("Junctions backward");
                pathManager.DebugPrintJunctions(pathManager.reverseJunctions, pathManager.reversePath);
            }
            if (GUILayout.Button("Debug Print Pathing"))
            {
                pathManager.DebugPrintPathing();
            }
        }
    }
}
