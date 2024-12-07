using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CrossingInfo
{
    public string Name;
    public bool State;
    public List<(int Index, string Name)> AllowedStates;
    public CrossingSystem CrossingSystem;
}

public class CrossingSystem : MonoBehaviour
{
    [SerializeField] private UnityServerComm comm;
    [SerializeField] private SemaphoreCrossingsController semaphoreRoadController;
    [SerializeField] private List<SemaphoreCrossingShieldController> semaphoreCrossingShieldList;
    [SerializeField] private Animator animator;
    private Coroutine openingCoroutine;
    public string Name;
    public int Id = 0;
    [SerializeField] private float barrierDelay = 6.0f;
    [SerializeField] private bool damagedCrossing = false;

    public CrossingInfo crossingInfo;
    private List<(int Index, string Name)> allowedStatesList = new List<(int Index, string Name)> { (0,"Working"),(1,"Damaged")};

    public CrossingInfo GetCrossingInfo()
    {
        return new CrossingInfo()
        {
            Name = Name, State = damagedCrossing, AllowedStates = allowedStatesList, CrossingSystem = this 
        };
    }

    public int GetStateIndex()
    {
        if (damagedCrossing) return 1;
        else return 0;
    }

    private IEnumerator OpenCrossingEnumerator()
    {
        animator.SetBool("Close", false);
        yield return new WaitForSeconds(barrierDelay);
        semaphoreRoadController.StopBlinking();
        openingCoroutine = null;
    }

    public void OpenCrossing()
    {
        if (openingCoroutine == null && !damagedCrossing)
        {
            openingCoroutine = StartCoroutine(OpenCrossingEnumerator());
        }
    }

    public void CloseCrossing()
    {
        if (!damagedCrossing)
        {
            if (openingCoroutine != null)
            {
                StopCoroutine(openingCoroutine);
                openingCoroutine = null;
            }
            animator.SetBool("Close", true);
            semaphoreRoadController.StartBlinking();
        }
    }

    void Start()
    {
        //allowedStatesList = GetAllowedStates();
        crossingInfo = GetCrossingInfo();

        if (damagedCrossing)
        {
            SendCrossingStateToServer();
        }
    }

    public void SetCrossingShieldSemaphoresState(int stateIdx)
    {
        foreach (SemaphoreController semaphoreController in semaphoreCrossingShieldList)
        {
            ISemaphoreState state = semaphoreController.allowedStatesList[stateIdx].Item2;
            semaphoreController.SetState(state);
        }
    }

    public void ChangeCrossingState(int state)
    {
        if (state == 0)
        {
            damagedCrossing = false;
            SetCrossingShieldSemaphoresState(0);
        }
        else
        {
            damagedCrossing = true;
            SetCrossingShieldSemaphoresState(1);
        }
        SendCrossingStateToServer();
    }

    void FixedUpdate()
    {

    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SendCrossingStateToServer()
    {
        comm.SendCrossingState(Id, !damagedCrossing);
    }
}
