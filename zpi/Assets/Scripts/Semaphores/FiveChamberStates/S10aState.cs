using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S10aState : ISemaphoreState
{
    //string name = "S10a 60km/h Next Max";
    string name = "S10a";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(0, controller.greenLight);
        controller.SetLight(3, controller.orangeLight);
        controller.SetLight(5, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
