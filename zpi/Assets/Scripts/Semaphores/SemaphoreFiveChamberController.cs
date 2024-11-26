using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SemaphoreFiveChamberController : SemaphoreController
{
    //public override void SetSignal(int signalIndex)
    //{
    //    SemaphoreFiveChamberSignals signal = (SemaphoreFiveChamberSignals)signalIndex;
    //    if ((int)signal != currentSignal)
    //    {
    //        StopBlinkLight();
    //        ResetLights();

    //        switch (signal)
    //        {
    //            case SemaphoreFiveChamberSignals.S1:
    //                SetLight(2, redLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S2:
    //                SetLight(0, greenLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S4:
    //                StartBlinkLight(1, orangeLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S5:
    //                SetLight(1, orangeLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S10:
    //                SetLight(0, greenLight);
    //                SetLight(3, orangeLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S12:
    //                StartBlinkLight(1, orangeLight);
    //                SetLight(3, orangeLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S13:
    //                SetLight(1, orangeLight);
    //                SetLight(3, orangeLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S10a:
    //                SetLight(0, greenLight);
    //                SetLight(3, orangeLight);

    //                SetLight(5, orangeLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S12a:
    //                StartBlinkLight(1, orangeLight);
    //                SetLight(3, orangeLight);

    //                SetLight(5, orangeLight);
    //                break;
    //            case SemaphoreFiveChamberSignals.S13a:
    //                SetLight(1, orangeLight);
    //                SetLight(3, orangeLight);

    //                SetLight(5, orangeLight);
    //                break;
    //            default:
    //                Debug.Log("Bad signal " + signal);
    //                break;
    //        }
    //        currentSignal = (int)signal;
    //    }
    //}

    override public List<(int, ISemaphoreState)> GetAllowedStates()
    {
        List<(int, ISemaphoreState)> allowedStates = new List<(int, ISemaphoreState)>();

        Assembly assembly = Assembly.Load("FiveChamberStates");

        if (assembly == null)
        {
            Debug.LogError("Could not load Assembly.");
            return allowedStates;
        }

        List<Type> stateTypeList = assembly.GetTypes()
            .Where(t => typeof(ISemaphoreState).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .ToList();

        int i = 0;
        foreach (Type type in stateTypeList)
        {
            ISemaphoreState state = (ISemaphoreState)Activator.CreateInstance(type);
            allowedStates.Add((i, state));
        }

        return allowedStates;
    }

    public override void SendSemaphoreInfoToSever()
    {
        comm.SendSemaphoreSignal(0, shouldGo());
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
