using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
[CreateAssetMenu(fileName = "New spline decorations", menuName = "Spline decorations")]
public class SplineDecorationObjects : ScriptableObject
{
    public List<SplineAndDecorations> instantiatedObjects = new List<SplineAndDecorations>();
}

[Serializable]
public class SplineAndDecorations
{
    public Spline spline;
    public List<GameObject> decorations;
}