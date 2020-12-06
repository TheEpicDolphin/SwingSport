using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFollowingCameraFollowee
{
    Vector3 Position();

    void Draw();
}

public class FollowingCamera : MonoBehaviour
{
    /* mouse position on screen */
    float mouseX, mouseY;

    /* mouse sensitivity */
    float mouseSensitivity = 5.0f;

    /* How responsive the camera feels. [0, 1]*/
    float cameraFluidity = 0.8f;

    public IFollowingCameraFollowee followee;

    /* camera is a child of view. view is always constrained to the position of the followee */
    Transform view;

    Camera cam;

    Quaternion targetRotation = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        view = transform.parent;
        cam = view.GetComponentInChildren<Camera>();
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
        view.position = followee.Position();
        view.rotation = Quaternion.Slerp(view.rotation, targetRotation, cameraFluidity);
    }

    private void OnPostRender()
    {
        followee.Draw();
    }

    public Ray LineOfSightRay()
    {
        return cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    }
}
