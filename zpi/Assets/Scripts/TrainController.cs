using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainController : MonoBehaviour
{
    public int framesToStop;
    public SplineContainer jumpTo;

    private void FixedUpdate()
    {
        if(framesToStop == -1)
        {
            return;
        }

        framesToStop -= 1;
        if(framesToStop == 0)
        {
            Jump();
        }  
    }


    private void Stop()
    {
        //gameObject.GetComponent<SplineAnimate>().MaxSpeed = 1.0f;
        gameObject.GetComponent<SplineAnimate>().enabled = false;
    }


    private void Jump()
    {
        gameObject.GetComponent<SplineAnimate>().Container = jumpTo;
        gameObject.GetComponent<SplineAnimate>().Restart(true);
    }
}
