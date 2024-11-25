using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Os2State : ISemaphoreState
{
    //string name = "Os2 Max";
    string name = "Os2";
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
