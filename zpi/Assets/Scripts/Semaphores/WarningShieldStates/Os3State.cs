using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Os3State : ISemaphoreState
{
    string name = "Os3 100km/h";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(1, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
