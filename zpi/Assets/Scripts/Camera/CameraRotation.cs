using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{

    private float yRotation = 0f; // Rotation around the X axis
    private float xRotation = 0f; // Rotation around the Y axis
    [SerializeField] private float sensitivity = 2f; // Mouse sensitivity
    [SerializeField] private float maxXRotation = 360f;
    [SerializeField] private float maxYRotation = 90f;
    private Quaternion initialRotation;
    private Vector2 mouseDelta;



    void FixedUpdate()
    {
        initialRotation = transform.rotation;

        xRotation += mouseDelta.x * sensitivity;
        yRotation -= mouseDelta.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -maxXRotation, maxXRotation);
        yRotation = Mathf.Clamp(yRotation, -maxYRotation, maxYRotation);

        Quaternion mouseRotation = Quaternion.Euler(new Vector3(yRotation, xRotation, 0f));
        transform.rotation = mouseRotation * initialRotation;

        mouseDelta = Vector2.zero;
    }

    public void Look(Vector2 movement)
    {
        
        mouseDelta = movement;
        
    }
}
