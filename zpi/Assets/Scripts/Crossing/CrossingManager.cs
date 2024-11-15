using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossingManager : MonoBehaviour
{
    public List<CrossingSystem> crossings = new List<CrossingSystem>();

    public List<CrossingInfo> GetCrossingInfos()
    {
        List<CrossingInfo> crossingInfos = new List<CrossingInfo>();
        foreach (CrossingSystem crossing in crossings)
        {
            crossingInfos.Add(crossing.GetCrossingInfo());
        }
        return crossingInfos;
    }
}
