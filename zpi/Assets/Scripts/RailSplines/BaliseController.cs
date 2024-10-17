using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaliseController : MonoBehaviour
{
    public Balises balises;
    public int trackNumber;

    public int getBalisesGroupSize()
    {
        return balises.kilometers.Length;
    }

    public BaliseInfo checkForBalises(int lastBalise,float currentDistanceProc,bool speedPlus)
    {
        if (speedPlus && lastBalise != balises.kilometers.Length)
        {
            if ((float)balises.kilometers[lastBalise]/(float)balises.lenght < currentDistanceProc)
            {
                return new BaliseInfo() 
                { 
                    kilometer = balises.kilometers[lastBalise],
                    number = lastBalise+1,                   
                    groupSize = balises.kilometers.Length,
                    track = trackNumber,
                    line = 1
                };
            }
            else
            {
                return null;
            }
        }
        else if(lastBalise != 0)
        {
            if (balises.kilometers[lastBalise-1] / balises.lenght > currentDistanceProc)
            {
                return new BaliseInfo()
                {
                    kilometer = balises.kilometers[lastBalise-1],
                    number = lastBalise,
                    groupSize = balises.kilometers.Length,
                    track = trackNumber,
                    line = 1
                };
            }
            else
            {
                return null;
            }
        }
        return null;
    }
}
