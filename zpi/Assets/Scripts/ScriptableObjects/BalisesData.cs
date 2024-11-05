using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New balises",menuName ="Balises Data")]
[System.Serializable]
public class BalisesData : ScriptableObject
{
    public List<BaliseGroup> baliseGroups;
}

[System.Serializable]
public class BaliseGroup
{
    public float kilometer;
    public Balise[] balises;
}

[System.Serializable]
public class Balise
{
    public string messageType;
}
