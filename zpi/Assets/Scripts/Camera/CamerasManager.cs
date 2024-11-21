using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasManager : MonoBehaviour
{
    [SerializeField] private Camera[] cameras;
    [SerializeField] private Camera UICamera;
    [SerializeField] private Canvas ui;

    private int activeCameraIdx;

    private void Start()
    {
        if (cameras.Length > 0) 
        {
            cameras[0].enabled = true;
        }

        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].enabled = false;
        }
    }

    private void SwitchToCam(int cameraIdx)
    {
        if (cameraIdx < cameras.Length && cameraIdx != activeCameraIdx)
        {
            cameras[cameraIdx].enabled = true;
            cameras[activeCameraIdx].enabled = false;

            activeCameraIdx = cameraIdx;
        }
    }

    public void SwitchToNextCam()
    {
        Debug.Log("ye");
        int nextCamIdx = (activeCameraIdx + 1) % cameras.Length;
        SwitchToCam(nextCamIdx);
    }

    public Camera GetActiveCam() 
    { 
        return cameras[activeCameraIdx]; 
    }

    public void DisableUi()
    {
        UICamera.enabled = false;
    }

    public void EnableUi()
    {
        UICamera.enabled = true;
    }
}
