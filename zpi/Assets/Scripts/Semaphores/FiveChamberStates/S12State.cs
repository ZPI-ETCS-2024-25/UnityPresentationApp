using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S12State : ISemaphoreState
{
    //string name = "S12 40km/h Next 40 or 60km/h";
    string name = "S12";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.StartBlinkLight(1, controller.orangeLight);
        controller.SetLight(3, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
