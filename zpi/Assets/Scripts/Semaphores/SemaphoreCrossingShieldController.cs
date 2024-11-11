using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreCrossingShieldController : SemaphoreController
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
                case SemaphoreSignals.Osp1:
                    SetLight(2, orangeLight);
                    SetLight(3, orangeLight);
                    break;
                case SemaphoreSignals.Osp2:
                    SetLight(0, whiteLight);
                    SetLight(1, whiteLight);
                    break;
                default:
                    Debug.Log("Bad signal " + signal);
                    break;
            }
            prevSignal = signal;
        }
    }
}
