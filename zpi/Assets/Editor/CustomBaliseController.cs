using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaliseController))]
public class CustomBaliseController : Editor
{
    private bool showButtons = false;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BaliseController baliseController = (BaliseController)target;

        showButtons = EditorGUILayout.Foldout(showButtons, "Testing");

        if (showButtons)
        {
            if (GUILayout.Button("Create balises"))
            {
                baliseController.PlaceBalises();
            }
            if (GUILayout.Button("Remove balises"))
            {
                baliseController.RemoveBalises();
            }
        }
    }
}
