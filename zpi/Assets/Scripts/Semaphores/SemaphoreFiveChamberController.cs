using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SemaphoreFiveChamberController : SemaphoreController
{   override public List<(int, ISemaphoreState)> GetAllowedStates()
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
        comm.SendSemaphoreSignal(id, shouldGo());
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
