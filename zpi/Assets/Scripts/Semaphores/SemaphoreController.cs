using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public struct SemaphoreInfo
{
    public string Name;
    public int Signal;
    public List<(int Index, string Name)> AllowedSignals;
    public SemaphoreController SemaphoreController;
}

public abstract class SemaphoreController : MonoBehaviour
{

    [SerializeField] private GameObject[] lightLens;
    [SerializeField] protected Material blankLight;
    [SerializeField] protected Material redLight;
    [SerializeField] protected Material orangeLight;
    [SerializeField] protected Material greenLight;
    [SerializeField] protected Material whiteLight;

    [SerializeField] protected float blinkDelay = 1.0f;
    private bool isBlinking = true; //check to keep one coroutine running
    private Coroutine blinkingCoroutine; //reference for coroutine to stop it
    private Material blinkColor;
    private int blinkLensIndex;
    public int currentSignal;
    public string name = "semaphore";

    public SemaphoreInfo semaphoreInfo;
    protected List<(int, string)> allowedSignalsList;

    protected void ResetLights()
    {
        //Debug.Log("Reset");
        for (int i = 0; i < lightLens.Length; i++)
        {
            lightLens[i].GetComponent<MeshRenderer>().material = blankLight;
        }
    }

    protected void SetLight(int lightIndex, Material color)
    {
        //Debug.Log(lightIndex + color.name);
        lightLens[lightIndex].GetComponent<MeshRenderer>().material = color;
    }

    private IEnumerator BlinkLight(int lightIndex, Material color)
    {
        isBlinking = true;
        SetLight(lightIndex, color);
        yield return new WaitForSeconds(blinkDelay);

        SetLight(lightIndex, blankLight);
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

    public virtual void SetSignal(int signalIndex)
    {
        Debug.Log("Not implemented");
    }

    virtual public List<(int, string)> GetAllowedSignals()
    {
        return new List<(int, string)>();
    }

    public SemaphoreInfo GetSemaphoreInfo()
    {
        return new SemaphoreInfo()
        {
            Name = name,
            Signal = currentSignal,
            AllowedSignals = allowedSignalsList,
            SemaphoreController = this
        };
    }

    private void Awake()
    {
        allowedSignalsList = GetAllowedSignals();
        semaphoreInfo = GetSemaphoreInfo();
        //Debug.Log($"{name}");
        //Debug.Log($"pre {allowedSignalsList}");
        //allowedSignalsList = semaphoreInfo.AllowedSignals;
        //Debug.Log($"post {allowedSignalsList}");
    }

    private void Start()
    {
        blinkLensIndex = 0;
        blinkColor = blankLight;
        /*SemaphoreInfo test = GetSemaphoreInfo();
        Debug.Log(test.Name);
        foreach ((int idx, string nam) in test.AllowedSignals)
        {
            Debug.Log(idx + " " + nam);
        }*/
    }

    private void FixedUpdate()
    {
        //SetSignal(currentSignal);
        if (!isBlinking)
        {
            blinkingCoroutine = StartCoroutine(BlinkLight(blinkLensIndex, blinkColor));
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}

//public enum SemaphoreSignals{
//    S1,
//    S2,
//    S4,
//    S5,
//    S10,
//    S10a,
//    S12,
//    S12a,
//    S13,
//    S13a,
//    Os1,
//    Os2, 
//    Os3, 
//    Os4,
//    Osp1,
//    Osp2,
//}

