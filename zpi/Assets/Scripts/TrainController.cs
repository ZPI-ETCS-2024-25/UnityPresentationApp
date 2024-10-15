using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class TrainController : MonoBehaviour
{
    public int framesToStop;
    public SplineContainer jumpTo;
    public bool shutUp;

    private void FixedUpdate()
    {
        if(!shutUp)
        {
            if (framesToStop == -1)
            {
                return;
            }

            framesToStop -= 1;
            if (framesToStop == 0)
            {
                Reverse();
            }
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


    private void Reverse()
    {
        //gameObject.GetComponent<SplineAnimate>().Container.Spline.Closed;
    }
}
