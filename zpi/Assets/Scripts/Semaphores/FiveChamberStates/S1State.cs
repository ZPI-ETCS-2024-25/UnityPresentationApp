using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S1State : ISemaphoreState
{
    //string name = "S01 Stop";
    string name = "S01";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(2, controller.redLight);
    }

    public bool ShouldGo()
    {
        return false;
    }
}
