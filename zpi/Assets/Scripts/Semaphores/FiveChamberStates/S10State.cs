using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S10State : ISemaphoreState
{
    //string name = "S10 40km/h Next Max";
    string name = "S10";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(0, controller.greenLight);
        controller.SetLight(3, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
