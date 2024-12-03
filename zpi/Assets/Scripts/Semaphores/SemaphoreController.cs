using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public struct SemaphoreInfo
{
    public string Name;
    public ISemaphoreState State;
    public List<(int Index, ISemaphoreState Name)> AllowedStates;
    public SemaphoreController SemaphoreController;
}

public abstract class SemaphoreController : MonoBehaviour
{
    [SerializeField] public UnityServerComm comm;
    [SerializeField] private GameObject[] lightLens;
    [SerializeField] protected Material blankLight;
    [SerializeField] public Material redLight;
    [SerializeField] public Material orangeLight;
    [SerializeField] public Material greenLight;
    [SerializeField] public Material whiteLight;

    [SerializeField] protected float blinkDelay = 1.0f;
    private bool isBlinking = true; //check to keep one coroutine running
    private Coroutine blinkingCoroutine; //reference for coroutine to stop it
    private Material blinkColor;
    private int blinkLensIndex;
    public string name = "semaphore";
    public int id;
    public ISemaphoreState currentState;

    public SemaphoreInfo semaphoreInfo;
    public List<(int, ISemaphoreState)> allowedStatesList;

    protected void ResetLights()
    {
        //Debug.Log("Reset");
        for (int i = 0; i < lightLens.Length; i++)
        {
            lightLens[i].GetComponent<MeshRenderer>().material = blankLight;
        }
    }

    public void SetLight(int lightIndex, Material color)
    {
        //Debug.Log(lightIndex + color.name);
        if (lightIndex > lightLens.Length)
        {
            return;
        }
        lightLens[lightIndex].GetComponent<MeshRenderer>().material = color;
    }

    private IEnumerator BlinkLight(int lightIndex, Material color)
    {
        if (lightIndex > lightLens.Length)
        {
            yield break;
        }
        isBlinking = true;
        SetLight(lightIndex, color);
        yield return new WaitForSeconds(blinkDelay);

        SetLight(lightIndex, blankLight);
        yield return new WaitForSeconds(blinkDelay);
        isBlinking = false;
    }

    public void StartBlinkLight(int lightIndex, Material color)
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

    public void SetState(ISemaphoreState state)
    {
        StopBlinkLight();
        ResetLights();

        state.SetSignal(this);
        currentState = state;
        SendSemaphoreInfoToSever();
    }

    virtual public List<(int, ISemaphoreState)> GetAllowedStates()
    {
        return new List<(int, ISemaphoreState)>();
    }

    public SemaphoreInfo GetSemaphoreInfo()
    {
        return new SemaphoreInfo()
        {
            Name = name,
            State = currentState,
            AllowedStates = allowedStatesList,
            SemaphoreController = this
        };
    }

    private void Awake()
    {
        allowedStatesList = GetAllowedStates();
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

    public abstract void SendSemaphoreInfoToSever();
    

    public bool shouldGo()
    {
        return currentState.ShouldGo();
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

