using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S13State : ISemaphoreState
{
    //string name = "S13 40km/h Next Stop";
    string name = "S13";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(1, controller.orangeLight);
        controller.SetLight(3, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
