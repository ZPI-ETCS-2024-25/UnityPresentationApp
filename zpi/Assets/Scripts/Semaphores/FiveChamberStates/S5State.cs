using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S5State : ISemaphoreState
{
    //string name = "S05 Next Stop";
    string name = "S05";
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
