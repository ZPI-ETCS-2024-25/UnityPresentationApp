using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreWarningShieldController : SemaphoreController
{
    override public void SetSignal(SemaphoreSignals signal)
    {
        if (signal != prevSignal)
        {
            StopBlinkLight();
            ResetLights();
            Debug.Log(signal);
            switch (signal)
            {
                case SemaphoreSignals.Os1:
                    SetLight(1, orangeLight);
                    break;
                case SemaphoreSignals.Os2:
                    SetLight(0, greenLight);
                    break;
                case SemaphoreSignals.Os3:
                    StartBlinkLight(0, greenLight);
                    break;
                case SemaphoreSignals.Os4:
                    StartBlinkLight(1, orangeLight);
                    break;
                default:
                    Debug.Log("Bad signal " + signal);
                    break;
            }
            prevSignal = signal;
        }
    }
}
