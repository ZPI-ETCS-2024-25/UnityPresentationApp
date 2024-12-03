using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S02State : ISemaphoreState
{
    //string name = "S02 Max";
    string name = "S02";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.SetLight(0, controller.greenLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
