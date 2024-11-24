using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Osp1State : ISemaphoreState
{
    //string name = "Osp1 20 km/h";
    string name = "Osp1";
    public string GetName()
    {
        return name;
    }
    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(2, controller.orangeLight);
        controller.SetLight(3, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
