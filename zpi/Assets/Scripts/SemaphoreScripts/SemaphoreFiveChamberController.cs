using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreFiveChamberController : SemaphoreController
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
                case SemaphoreSignals.S1:
                    SetLight(2, redLight);
                    break;
                case SemaphoreSignals.S2:
                    SetLight(0, greenLight);
                    break;
                case SemaphoreSignals.S4:
                    StartBlinkLight(1, orangeLight);
                    break;
                case SemaphoreSignals.S5:
                    SetLight(1, orangeLight);
                    break;
                case SemaphoreSignals.S10:
                    SetLight(0, greenLight);
                    SetLight(3, orangeLight);
                    break;
                case SemaphoreSignals.S12:
                    StartBlinkLight(1, orangeLight);
                    SetLight(3, orangeLight);
                    break;
                case SemaphoreSignals.S13:
                    SetLight(1, orangeLight);
                    SetLight(3, orangeLight);
                    break;
                case SemaphoreSignals.S10a:
                    SetLight(0, greenLight);
                    SetLight(3, orangeLight);

                    SetLight(5, orangeLight);
                    break;
                case SemaphoreSignals.S12a:
                    StartBlinkLight(1, orangeLight);
                    SetLight(3, orangeLight);

                    SetLight(5, orangeLight);
                    break;
                case SemaphoreSignals.S13a:
                    SetLight(1, orangeLight);
                    SetLight(3, orangeLight);

                    SetLight(5, orangeLight);
                    break;
                default:
                    Debug.Log("Bad signal " + signal);
                    break;
            }
            prevSignal = signal;
        }
    }
}
