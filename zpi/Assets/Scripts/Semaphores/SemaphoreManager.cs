using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreManager : MonoBehaviour
{
    public List<SemaphoreController> semaphores = new List<SemaphoreController> ();

    public List<SemaphoreInfo> GetSempahoreInfos()
    {
        List<SemaphoreInfo> semaphoreInfos = new List<SemaphoreInfo>();
        foreach (SemaphoreController semaphore in semaphores)
        {
            semaphoreInfos.Add(semaphore.GetSemaphoreInfo());
        }
        return semaphoreInfos;
    }
}
