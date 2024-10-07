using UnityEngine;
using UnityEditor;
using System;

public class CustomAddPathTool : EditorWindow
{
    GameObject objectWithPath;
    int numberOfNodes;
    int endIndexStart;


    [MenuItem("Tools/Add Path Tool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CustomAddPathTool));
    }


    private void OnGUI()
    {
        GUILayout.Label("Add path nodes", EditorStyles.boldLabel);
         
        objectWithPath = EditorGUILayout.ObjectField("Select obecjt with MoveAlongPath component",objectWithPath,typeof(GameObject),false) as GameObject;
        numberOfNodes = EditorGUILayout.IntField("How many nodes to add", numberOfNodes);
        endIndexStart = EditorGUILayout.IntField("Starting number to add at end of node name", endIndexStart);

        if(GUILayout.Button("Add path"))
        {
            AddPath();
        }
    }


    private void AddPath()
    {
        MoveAlongPath component = objectWithPath.GetComponent<MoveAlongPath>();

        if(component == null)
        {
            return;
        }


        int startingLenght = component.waypoints.Length;
        Array.Resize(ref component.waypoints, startingLenght + numberOfNodes);

    }
}
