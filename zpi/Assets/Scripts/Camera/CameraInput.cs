using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.CameraActions actions;

    private CamerasManager manager;

    private void Awake()
    {
        playerInput = new PlayerInput();
        actions = playerInput.Camera;
        manager = GetComponent<CamerasManager>();

        actions.Changecamera.performed += ctx => manager.SwitchToNextCam();
    }

    private void FixedUpdate()
    {

    }
    private void LateUpdate()
    {

    }
    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }
}
