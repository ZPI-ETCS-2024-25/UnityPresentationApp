using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreWarningShieldController : SemaphoreController
{
    private SemaphoreWarningShieldSignals prevSignal;
    public SemaphoreWarningShieldSignals currSignal;
    public void SetSignal(SemaphoreWarningShieldSignals signal)
    {
        if ((int)signal != currentSignal)
        {
            StopBlinkLight();
            ResetLights();

            switch (signal)
            {
                case SemaphoreWarningShieldSignals.Os1:
                    SetLight(1, orangeLight);
                    break;
                case SemaphoreWarningShieldSignals.Os2:
                    SetLight(0, greenLight);
                    break;
                case SemaphoreWarningShieldSignals.Os3:
                    StartBlinkLight(0, greenLight);
                    break;
                case SemaphoreWarningShieldSignals.Os4:
                    StartBlinkLight(1, orangeLight);
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
public enum SemaphoreWarningShieldSignals
{
    Os1,
    Os2,
    Os3,
    Os4
}