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

    public Player player;

    /* camera is a child of view. view is always constrained to at the position of the player */
    Transform view;

    Quaternion targetRotation = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        view = transform.parent;
    }

    public void UpdateCameraTargetRotation(float mouseXDelta, float mouseYDelta)
    {
        mouseX += mouseXDelta * mouseSensitivity;
        mouseY += mouseYDelta * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -80, 80);
        targetRotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }

    private void FixedUpdate()
    {
        view.position = player.transform.position;
        view.rotation = Quaternion.Slerp(view.rotation, targetRotation, cameraFluidity);
    }

    private void OnPostRender()
    {
        player.Draw();
    }
}
