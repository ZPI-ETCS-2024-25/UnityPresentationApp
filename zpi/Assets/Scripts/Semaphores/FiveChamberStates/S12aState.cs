using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S12aState : ISemaphoreState
{
    string name = "S12a 60km/h Next 40 or 60km/h";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.StartBlinkLight(1, controller.orangeLight);
        controller.SetLight(3, controller.orangeLight);
        controller.SetLight(5, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
