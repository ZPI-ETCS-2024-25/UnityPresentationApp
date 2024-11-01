using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.Splines;

public class BaliseController : MonoBehaviour
{
    public SplineContainer splineContainer;

    public List<BaliseInfo> checkForBalises(int splineIndex,float previousDistanceProc,float currentDistanceProc,int speedDirection)
    {
        SplineData<UnityEngine.Object> bD;
        SplineData<UnityEngine.Object> rD;
        bool gotBaliseData = splineContainer.Splines[splineIndex].TryGetObjectData("BaliseData", out bD);
        bool gotRailData = splineContainer.Splines[splineIndex].TryGetObjectData("RailData", out rD);
        BalisesData baliseData = bD[0].Value as BalisesData;
        RailData railData = rD[0].Value as RailData;


        if (!gotBaliseData || !gotRailData)
        {
            return null;
        }



        if ((speedDirection == 1 && previousDistanceProc < currentDistanceProc) || (speedDirection == -1 && previousDistanceProc > currentDistanceProc))
        {
            List<BaliseGroup> passedBalises = getPassedBalises(baliseData,railData,previousDistanceProc, currentDistanceProc,speedDirection);
            if(passedBalises.Count == 0)
            {
                return null;
            }
            List<BaliseInfo> passedBalisesInfo = getDataFromBalises(railData,passedBalises);
            if(passedBalisesInfo.Count > 0)
            {
                if (speedDirection == -1)
                {
                    passedBalisesInfo.Reverse();
                }
                return passedBalisesInfo;
            }
        }
        return null;
    }

    private List<BaliseGroup> getPassedBalises(BalisesData balisesData,RailData railData,float previousDistanceProc, float currentDistanceProc, int speedDirection) {
        List<BaliseGroup> passedBalises = new List<BaliseGroup>();
        foreach(BaliseGroup baliseGroup in balisesData.baliseGroups)
        {
            float baliseGroupPlacement = baliseGroup.kilometer / Math.Abs(railData.startKilometers-railData.endKilometers);
            if(speedDirection == 1)
            {
                if (previousDistanceProc <= baliseGroupPlacement && currentDistanceProc >= baliseGroupPlacement)
                {
                    passedBalises.Add(baliseGroup);
                }
            }
            if (speedDirection == -1)
            {
                if (currentDistanceProc <= baliseGroupPlacement && previousDistanceProc >= baliseGroupPlacement)
                {
                    passedBalises.Add(baliseGroup);
                }
            }

        }
        return passedBalises;
    }


    private List<BaliseInfo> getDataFromBalises(RailData railData,List<BaliseGroup> passedBalises)
    {
        List<BaliseInfo> passedBalisesInfo = new List<BaliseInfo>();
        foreach (BaliseGroup baliseGroup in passedBalises)
        {
            for (int i = 0; i < baliseGroup.balises.Length; i++)
            {
                passedBalisesInfo.Add(new BaliseInfo()
                {
                    kilometer = Convert.ToString(railData.startKilometers + baliseGroup.kilometer),
                    number = i,
                    numberOfBalises = baliseGroup.balises.Length,
                    trackNumber = railData.trackNumber,
                    lineNumber = railData.lineNumber,
                    messageType = baliseGroup.balises[i].messageType
                });
            }
        }
        return passedBalisesInfo;
    }

}
