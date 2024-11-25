using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Os4State : ISemaphoreState
{
    //string name = "Os4 40 or 60km/h";
    string name = "Os4";
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
