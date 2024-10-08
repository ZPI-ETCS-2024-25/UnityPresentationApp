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
         
        objectWithPath = EditorGUILayout.ObjectField("Select obecjt with MoveAlongPath component",objectWithPath,typeof(GameObject),true) as GameObject;
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


        int startingLenght;

        if (component.waypoints == null)
        {
            component.waypoints = new Transform[numberOfNodes];
            startingLenght = 0;
        }
        else
        {
            startingLenght = component.waypoints.Length;
            Array.Resize(ref component.waypoints, startingLenght + numberOfNodes);
        }


        int index = endIndexStart;
        int shift = 1;
        for(int i = startingLenght;i < startingLenght + numberOfNodes; i++)
        {
            Vector3 startingPosition = new Vector3();

            if(startingLenght == 0)
            {
                startingPosition = objectWithPath.transform.position;
            }
            else
            {
                startingPosition = component.waypoints[startingLenght-1].position;
            }

            GameObject node = new GameObject("node"+index);
            node.transform.position = startingPosition + new Vector3(5*shift,0,0);
            component.waypoints[i] = node.GetComponent<Transform>();
            index++;
            shift++;
        }
        
        

    }
}
