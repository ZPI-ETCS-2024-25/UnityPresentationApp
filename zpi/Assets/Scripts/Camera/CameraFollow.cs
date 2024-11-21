using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 posOffset;
    private Quaternion rotOffset;
    private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime; //distance follow smoothing amount
    [SerializeField] private float rotationSpeed; //rotation follow smothing amount
    [SerializeField] private Vector3 cameraPosOffset;
    [SerializeField] private float cameraXAngle;

    private void CalculateCameraPosition()
    {
        Vector3 targetPosition = target.position + target.rotation * cameraPosOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }

    private void CalculateCameraRotation(float timeType)
    {
        Quaternion targetRotation = target.rotation * rotOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * timeType);
    }

    private void Awake()
    {
        rotOffset = Quaternion.Euler(cameraXAngle,0,0);
    }

    private void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    private void FixedUpdate()
    {
        CalculateCameraPosition();
        CalculateCameraRotation(Time.fixedDeltaTime);
    }
}
