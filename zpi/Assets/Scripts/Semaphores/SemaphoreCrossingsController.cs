using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SemaphoreCrossingsController : SemaphoreController
{
    private bool blinking = true; //check to keep one coroutine running
    private Coroutine blinkingCoroutine;


    private IEnumerator BlinkCrossingLights()
    {
        blinking = true;
        SetLight(0, redLight);
        SetLight(1, blankLight);
        SetLight(2, redLight);
        SetLight(3, blankLight);
        yield return new WaitForSeconds(blinkDelay);

        SetLight(0, blankLight);
        SetLight(1, redLight);
        SetLight(2, blankLight);
        SetLight(3, redLight);
        yield return new WaitForSeconds(blinkDelay);
        blinkingCoroutine = null;
        blinking = false;
    }

    public void StartBlinking()
    {
        if (blinkingCoroutine == null)
        {
            blinking = false;
        }
    }
    public void StopBlinking()
    {
        if (blinkingCoroutine != null)
        {
            StopCoroutine(blinkingCoroutine);
            blinkingCoroutine = null;
        }
        blinking = true;
        ResetLights();
    }

    void FixedUpdate()
    {
        if (!blinking && blinkingCoroutine == null)
        {
            blinkingCoroutine = StartCoroutine(BlinkCrossingLights());
        }
    }

    public override bool shouldGo()
    {
        throw new System.Exception("Calling should go for crossing!");
    }
}
