using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LenghtManager))]
public class CustomLenghtManager : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        LenghtManager lenghtManager = (LenghtManager)target;


        if (GUILayout.Button("Get modifier"))
        {
            lenghtManager.GetModifier();
        }

        if (GUILayout.Button("Get real kilomets"))
        {
            lenghtManager.ProvideKilometersInfo();
        }
    }
}
