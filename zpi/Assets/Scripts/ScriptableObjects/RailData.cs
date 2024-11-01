using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New rail data", menuName = "Rail Data")]
public class RailData : ScriptableObject
{
    public string trackNumber;
    public int lineNumber;
    public float startKilometers;
    public float endKilometers;
}
