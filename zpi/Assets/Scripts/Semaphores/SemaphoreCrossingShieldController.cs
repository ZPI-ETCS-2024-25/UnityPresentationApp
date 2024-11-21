using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreCrossingShieldController : SemaphoreController
{
    public override void SetSignal(int signalIndex)
    {
        SemaphoreCrossingShieldSignals signal = (SemaphoreCrossingShieldSignals)signalIndex;
        if ((int)signal != currentSignal)
        {
            StopBlinkLight();
            ResetLights();

            switch (signal)
            {
                case SemaphoreCrossingShieldSignals.Osp1:
                    SetLight(2, orangeLight);
                    SetLight(3, orangeLight);
                    break;
                case SemaphoreCrossingShieldSignals.Osp2:
                    SetLight(0, whiteLight);
                    SetLight(1, whiteLight);
                    break;
                default:
                    Debug.Log("Bad signal " + signal);
                    break;
            }
            currentSignal = (int)signal;
        }
    }

    override public List<(int, string)> GetAllowedSignals()
    {
        List<(int, string)> allowedSignals = new List<(int, string)>();

        foreach (SemaphoreCrossingShieldSignals signal in Enum.GetValues(typeof(SemaphoreCrossingShieldSignals)))
        {
            allowedSignals.Add(((int)signal, signal.ToString()));
        }

        return allowedSignals;
    }

    public override bool shouldGo()
    {
        return ((SemaphoreCrossingShieldSignals)currentSignal == SemaphoreCrossingShieldSignals.Osp1 ? true : false);
    }
}

public enum SemaphoreCrossingShieldSignals
{
    Osp1,
    Osp2,
}
