using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LenghtManager : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float modifier;

    public void GetModifier()
    {
        SplineData<UnityEngine.Object> rD;
        bool gotRailData = splineContainer.Splines[0].TryGetObjectData("RailData", out rD);
        RailData railData = rD[0].Value as RailData;

        modifier = Mathf.Abs(railData.endKilometers - railData.startKilometers) / splineContainer.CalculateLength(0);
        railData.realKilometers = Mathf.Abs(railData.endKilometers - railData.startKilometers);
        railData.inputedKilometers = railData.realKilometers;
    }

    public void ProvideKilometersInfo()
    {
        for(int i = 1; i < splineContainer.Splines.Count; i++)
        {
            SplineData<UnityEngine.Object> rD;
            bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
            RailData railData = rD[0].Value as RailData;

            railData.realKilometers = splineContainer.CalculateLength(i) * modifier;
            railData.inputedKilometers = Mathf.Abs(railData.endKilometers - railData.startKilometers);
        }
    }
}
