using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S13aState : ISemaphoreState
{
    string name = "S13a 60km/h Next Stop";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(1, controller.orangeLight);
        controller.SetLight(3, controller.orangeLight);
        controller.SetLight(5, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
