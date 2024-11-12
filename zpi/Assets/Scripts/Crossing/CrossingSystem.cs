using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private IEnumerator OpenCrossingEnumerator()
    {
        animator.SetBool("Close", false);
        yield return new WaitForSeconds(barrierDelay);
        semaphoreController.StopBlinking();
        openingCoroutine = null;
    }

    public void OpenCrossing()
    {
        if (openingCoroutine == null)
        {
            openingCoroutine = StartCoroutine(OpenCrossingEnumerator());
        }
    }

    public void CloseCrossing()
    {
        if (openingCoroutine != null)
        {
            StopCoroutine(openingCoroutine);
            openingCoroutine = null;
        }
        animator.SetBool("Close", true);
        semaphoreController.StartBlinking();
    }

    void Start()
    {
        if (damagedCrossing)
        {
            comm.SendCrossingState(0, damagedCrossing);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (close) {
            CloseCrossing();
            close = false;
        }if (open) {
            OpenCrossing(); 
            open = false; 
        }
    }
}
