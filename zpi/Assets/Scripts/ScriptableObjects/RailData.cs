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
    public int junctionGroup = 0;
    public int junctionNumber = 0;
    public int straightIndex = 0;
}
