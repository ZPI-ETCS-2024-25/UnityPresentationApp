using PlasticGui.Configuration.CloudEdition.Welcome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SemaphoreWarningShieldController : SemaphoreController
{
    //public override void SetSignal(int signalIndex)
    //{
    //    SemaphoreWarningShieldSignals signal = (SemaphoreWarningShieldSignals)signalIndex;
    //    if ((int)signal != currentSignal)
    //    {
    //        StopBlinkLight();
    //        ResetLights();

    //        switch (signal)
    //        {
    //            case SemaphoreWarningShieldSignals.Os1:
    //                SetLight(1, orangeLight);
    //                break;
    //            case SemaphoreWarningShieldSignals.Os2:
    //                SetLight(0, greenLight);
    //                break;
    //            case SemaphoreWarningShieldSignals.Os3:
    //                StartBlinkLight(0, greenLight);
    //                break;
    //            case SemaphoreWarningShieldSignals.Os4:
    //                StartBlinkLight(1, orangeLight);
    //                break;
    //        }
    //        currentSignal = (int)signal;
    //    }
    //}

    override public List<(int, ISemaphoreState)> GetAllowedStates()
    {
        List<(int, ISemaphoreState)> allowedStates = new List<(int, ISemaphoreState)>();

        var assembly = Assembly.Load("WarningShieldStates");

        if (assembly == null)
        {
            Debug.LogError("Could not load Assembly.");
            return allowedStates;
        }

        var stateTypeList = assembly.GetTypes()
            .Where(t => typeof(ISemaphoreState).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .ToList();

        int i = 0;
        foreach (var type in stateTypeList)
        {
            ISemaphoreState state = (ISemaphoreState)Activator.CreateInstance(type);
            allowedStates.Add((i, state));
        }

        return allowedStates;
    }

    private void Start()
    {
        Debug.Log(this.name);
        foreach (var x in GetAllowedStates())
        {
            Debug.Log(x);
        }
        
    }
}
public enum SemaphoreWarningShieldSignals
{
    Os1,
    Os2,
    Os3,
    Os4
}