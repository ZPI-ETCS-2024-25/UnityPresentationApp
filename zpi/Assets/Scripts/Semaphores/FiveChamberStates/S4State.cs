using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S4State : ISemaphoreState
{
    string name = "S04 Next 40 or 60km/h";
    public string GetName()
    {
        return name;
    }

    public void SetSignal(SemaphoreController controller)
    {
        controller.StartBlinkLight(1, controller.orangeLight);
    }

    public bool ShouldGo()
    {
        return true;
    }
}
