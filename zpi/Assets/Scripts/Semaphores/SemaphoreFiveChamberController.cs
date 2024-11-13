using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreFiveChamberController : SemaphoreController
{
    public void SetSignal(SemaphoreFiveChamberSignals signal)
    {
        if ((int)signal != currentSignal)
        {
            StopBlinkLight();
            ResetLights();

            switch (signal)
            {
                case SemaphoreFiveChamberSignals.S1:
                    SetLight(2, redLight);
                    break;
                case SemaphoreFiveChamberSignals.S2:
                    SetLight(0, greenLight);
                    break;
                case SemaphoreFiveChamberSignals.S4:
                    StartBlinkLight(1, orangeLight);
                    break;
                case SemaphoreFiveChamberSignals.S5:
                    SetLight(1, orangeLight);
                    break;
                case SemaphoreFiveChamberSignals.S10:
                    SetLight(0, greenLight);
                    SetLight(3, orangeLight);
                    break;
                case SemaphoreFiveChamberSignals.S12:
                    StartBlinkLight(1, orangeLight);
                    SetLight(3, orangeLight);
                    break;
                case SemaphoreFiveChamberSignals.S13:
                    SetLight(1, orangeLight);
                    SetLight(3, orangeLight);
                    break;
                case SemaphoreFiveChamberSignals.S10a:
                    SetLight(0, greenLight);
                    SetLight(3, orangeLight);

                    SetLight(5, orangeLight);
                    break;
                case SemaphoreFiveChamberSignals.S12a:
                    StartBlinkLight(1, orangeLight);
                    SetLight(3, orangeLight);

                    SetLight(5, orangeLight);
                    break;
                case SemaphoreFiveChamberSignals.S13a:
                    SetLight(1, orangeLight);
                    SetLight(3, orangeLight);

                    SetLight(5, orangeLight);
                    break;
                default:
                    Debug.Log("Bad signal " + signal);
                    break;
            }
            currentSignal = (int)signal;
        }
    }

    override protected List<(int, string)> GetAllowedSignals()
    {
        List<(int, string)> allowedSignals = new List<(int, string)>();

        foreach (SemaphoreFiveChamberSignals signal in Enum.GetValues(typeof(SemaphoreFiveChamberSignals)))
        {
            allowedSignals.Add(((int)signal, signal.ToString()));
        }

        return allowedSignals;
    }
}
public enum SemaphoreFiveChamberSignals
{
    S1,
    S2,
    S4,
    S5,
    S10,
    S10a,
    S12,
    S12a,
    S13,
    S13a
}
