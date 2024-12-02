using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CamerasManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera topViewCamera;
    [SerializeField] private CinemachineCamera cabinViewCamera;
    [SerializeField] private LayerMask topLayerMask;
    [SerializeField] private LayerMask cabinLayerMask;

    public void SwitchView()
    {
        if (topViewCamera.Priority > cabinViewCamera.Priority)
        {
            cabinViewCamera.Priority = 10;
            topViewCamera.Priority = 0;

            mainCamera.cullingMask = cabinLayerMask;
        }
        else
        {
            topViewCamera.Priority = 10;
            cabinViewCamera.Priority = 0;

            mainCamera.cullingMask = topLayerMask;
        }
    }
}
