using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject spawnTarget;
    [SerializeField] private float speed = 10f; // Movement speed

    private PlayerInput playerInput;
    public PlayerInput.CameraActions actions;


    void onEnable()
    {
    //    transform.position = spawnTarget.position;
    //    transform.rotation = spawnTarget.rotation;
    }

    void Awake()
    {
        playerInput = new PlayerInput();
        actions = playerInput.Camera;
    }

    void Update()
    {

    }

    public void Movement(Vector3 movement)
    {
        //Vector3 movement = moveAction.ReadValue<Vector3>();
        transform.Translate(movement * speed * Time.deltaTime);
    }
}
