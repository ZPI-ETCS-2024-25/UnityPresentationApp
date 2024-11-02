using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SemaphoreController : MonoBehaviour
{

    [SerializeField] private GameObject[] lightLens;
    [SerializeField] private Material blankLight;
    [SerializeField] protected Material redLight;
    [SerializeField] protected Material orangeLight;
    [SerializeField] protected Material greenLight;
    [SerializeField] protected Material whiteLight;

    [SerializeField] private float blinkDelay = 1.0f;
    private bool isBlinking = false; //check to keep one coroutine running
    private Coroutine blinkingCoroutine; //reference for coroutine to stop it
    private Material blinkColor;
    private int blinkLensIndex;

    protected SemaphoreSignals prevSignal;
    public SemaphoreSignals currentSignal;

    protected void ResetLights()
    {
        Debug.Log("Reset");
        for (int i = 0; i < lightLens.Length; i++)
        {
            lightLens[i].GetComponent<MeshRenderer>().material = blankLight;
        }
    }

    protected void SetLight(int lightIndex, Material color)
    {
        Debug.Log(lightIndex + color.name);
        lightLens[lightIndex].GetComponent<MeshRenderer>().material = color;
    }

    private IEnumerator BlinkLight(int lightIndex, Material color)
    {
        isBlinking = true;
        lightLens[lightIndex].GetComponent<MeshRenderer>().material = color;
        yield return new WaitForSeconds(blinkDelay);

        lightLens[lightIndex].GetComponent<MeshRenderer>().material = blankLight;
        yield return new WaitForSeconds(blinkDelay);
        isBlinking = false;
    }

    protected void StartBlinkLight(int lightIndex, Material color)
    {
        blinkColor = color;
        blinkLensIndex = lightIndex;
        isBlinking = false;
    }
    protected void StopBlinkLight(){
        if (blinkingCoroutine != null)
        {
            StopCoroutine(blinkingCoroutine);
            blinkingCoroutine = null;
        }
        isBlinking = true;
    }

    virtual public void SetSignal(SemaphoreSignals signal){
        Debug.Log("Implement in subclass");
    }

    private void Start()
    {
        blinkLensIndex = 0;
        blinkColor = blankLight;
    }

    private void FixedUpdate()
    {
        SetSignal(currentSignal);
        if (!isBlinking)
        {
            blinkingCoroutine = StartCoroutine(BlinkLight(blinkLensIndex, blinkColor));
        }
    }
}

public enum SemaphoreSignals{
    S1,
    S2,
    S4,
    S5,
    S10,
    S10a,
    S12,
    S12a,
    S13,
    S13a,
    Os1,
    Os2, 
    Os3, 
    Os4,
    Osp1,
    Osp2,
}

