using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Os1State : ISemaphoreState
{
    string name = "Os1 Stop";
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
        return false;
    }

}
