using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    /* mouse position on screen */
    float mouseX, mouseY;

    /* mouse sensitivity */
    float mouseSensitivity = 5.0f;

    /* How responsive the camera feels. [0, 1]*/
    float cameraFluidity = 0.8f;

    public Transform ragdollTrans;

    private bool isThirdPerson;

    /* camera is a child of view. view is always constrained to at the position of the player */
    Transform view;

    Quaternion targetRotation = Quaternion.identity;

    public float firstPersonTurnSpeed = 4.0f;

    public float firstPersonMinTurnAngle = -90.0f;
    public float firstPersonMaxTurnAngle = 90.0f;
    private float firstPersonRotX;

    private Vector3 initialCameraEulerAngles;
    private Vector3 firstPersonToThirdPersonOffset;
    private Vector3 thirdPersonToFirstPersonOffset;

    public bool firstTimeCameraSwitch = true;

    // Start is called before the first frame update
    void Start()
    {
        view = transform.parent;
        initialCameraEulerAngles = transform.eulerAngles;

        firstPersonToThirdPersonOffset =
            1.4f * (Quaternion.AngleAxis(270, Vector3.up) * ragdollTrans.transform.forward) +
            3.0f * ragdollTrans.transform.forward;

    }

    public void UpdateCameraTargetRotation(float mouseXDelta, float mouseYDelta)
    {
        mouseX += mouseXDelta * mouseSensitivity;
        mouseY += mouseYDelta * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -80, 80);
        targetRotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }

    private void Update()
    {

        if (!isThirdPerson)
        {
            if (firstTimeCameraSwitch)
            {
                firstTimeCameraSwitch = false;
            }

            // TODO: Lerp camera to new position
            view.transform.position = ragdollTrans.position + firstPersonToThirdPersonOffset;

            FirstPersonMouseAiming();
        }
    }

    private void FixedUpdate()
    {
        
        if (isThirdPerson)
        {
            if (firstTimeCameraSwitch)
            {
                firstTimeCameraSwitch = false;

                transform.eulerAngles = initialCameraEulerAngles;

            }

            // TODO: Lerp camera to new position
            view.transform.position = ragdollTrans.position;

            view.rotation = Quaternion.Slerp(view.rotation, targetRotation, cameraFluidity);
        }
    }

    public void setThirdPerson(bool b)
    {
        isThirdPerson = b;
        firstTimeCameraSwitch = true;
    }

    void FirstPersonMouseAiming()
    {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * firstPersonTurnSpeed;
        firstPersonRotX += Input.GetAxis("Mouse Y") * firstPersonTurnSpeed;

        // clamp the vertical rotation
        firstPersonRotX = Mathf.Clamp(firstPersonRotX, firstPersonMinTurnAngle, firstPersonMaxTurnAngle);

        // rotate the camera
        transform.eulerAngles = new Vector3(-firstPersonRotX, transform.eulerAngles.y + y, 0);
    }

}