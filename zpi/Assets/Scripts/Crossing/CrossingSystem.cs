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
    [SerializeField] private SemaphoreCrossingsController semaphoreController;
    [SerializeField] private Animator animator;
    public bool close = false;
    public bool open = false;
    private Coroutine openingCoroutine;
    [SerializeField] private float barrierDelay = 6.0f;
    [SerializeField] private bool damagedCrossing = false;

    public CrossingInfo crossingInfo;
    private List<(int Index, string Name)> allowedStatesList;

    private List<(int Index, string Name)> GetAllowedStates()
    {
        List<(int, string)> allowedStates = new List<(int, string)>();
        allowedStates.Add((0, "Working"));
        allowedStates.Add((1, "Damaged"));

        return allowedStates;
    }

    public CrossingInfo GetCrossingInfo()
    {
        return new CrossingInfo()
        {
            Name = this.name, State = damagedCrossing, AllowedStates = allowedStatesList, CrossingSystem = this 
        };
    }

    private IEnumerator OpenCrossingEnumerator()
    {
        animator.SetBool("Close", false);
        yield return new WaitForSeconds(barrierDelay);
        semaphoreController.StopBlinking();
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
            semaphoreController.StartBlinking();
        }
    }

    void Start()
    {
        allowedStatesList = GetAllowedStates();
        crossingInfo = GetCrossingInfo();

        if (damagedCrossing)
        {
            comm.SendCrossingState(0, damagedCrossing);
        }
    }

    void FixedUpdate()
    {
        //if (close) {
        //    CloseCrossing();
        //    close = false;
        //}if (open) {
        //    OpenCrossing(); 
        //    open = false; 
        //}
    }
}
