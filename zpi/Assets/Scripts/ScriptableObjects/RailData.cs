using System;
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
    public float inputedKilometers;
    public float realKilometers;
    public int startingKnotGroup = 1;
    public int endingKnotGroup = 1;
    public JunctionData junctionDataForward;
    public JunctionData junctionDataBackward;
}



[Serializable]
public struct JunctionData
{
    public int junctionGroup;
    public int junctionNumber;
    public int straightIndex;
}