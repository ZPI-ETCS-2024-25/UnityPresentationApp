using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SemaphoreCrossingShieldController : SemaphoreController
{
    override public List<(int, ISemaphoreState)> GetAllowedStates()
    {
        List<(int, ISemaphoreState)> allowedStates = new List<(int, ISemaphoreState)>();

        var assembly = Assembly.Load("CrossingShieldStates");

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

    public override void SendSemaphoreInfoToSever()
    {
       
    }
}

public enum SemaphoreCrossingShieldSignals
{
    Osp1,
    Osp2,
}
