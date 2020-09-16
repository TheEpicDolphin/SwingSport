using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCameraController : MonoBehaviour
{

    /* mouse position on screen */
    float mouseX, mouseY;

    /* mouse sensitivity */
    float mouseSensitivity = 5.0f;

    public Transform ragdollTrans;

    /* camera is a child of view. view is always constrained to at the position of the player */
    Transform view;

    // Start is called before the first frame update
    void Start()
    {
        view = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -80, 80);
    }

    private void FixedUpdate()
    {
        view.position = ragdollTrans.position;
        view.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }
}
