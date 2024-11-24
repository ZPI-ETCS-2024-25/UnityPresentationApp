using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Osp2State : ISemaphoreState
{
    string name = "Osp2 Max";
    public string GetName()
    {
        return name;
    }
    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(0, controller.whiteLight);
        controller.SetLight(1, controller.whiteLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
