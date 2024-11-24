using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISemaphoreState
{
    void SetSignal(SemaphoreController controller);
    bool ShouldGo();
    string GetName();
}
