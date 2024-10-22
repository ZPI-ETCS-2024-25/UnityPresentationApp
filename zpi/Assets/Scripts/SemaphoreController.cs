using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreController : MonoBehaviour
{

    [SerializeField] private GameObject[] lightLens;
    [SerializeField] private Material blankLight;
    [SerializeField] private Material redLight;
    [SerializeField] private Material orangeLight;
    [SerializeField] private Material greenLight;

    [SerializeField] private float blinkDelay = 1.0f;
    private bool isBlinking = false;

    private void ResetLights()
    {
        for (int i = 0; i < lightLens.Length; i++)
        {
            lightLens[i].GetComponent<MeshRenderer>().material = blankLight;
        }
    }

    private void SetLight(int lightIndex, Material color)
    {
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
    // Start is called before the first frame update
    void Start()
    {
        ResetLights();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkLight(0, greenLight));
        }
    }


}
