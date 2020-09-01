using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Transform view;
    Transform camTarget;
    Transform hand;
    Camera mainCamera;
    float mouseX, mouseY;
    float viewRotationSpeed = 10.0f;
    float movementSpeed = 8.0f;
    float cameraRigidness = 10.0f;
    Vector3 movement = Vector3.zero;
    public bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = transform.Find("View");
        hand = transform.Find("Hand");
        camTarget = view.GetChild(0);
        mainCamera = Camera.main;
        mainCamera.transform.position = camTarget.transform.position;
        mainCamera.transform.rotation = camTarget.transform.rotation;

        //Instantiate hook gun in player's hand
        GameObject hookGunGO = (GameObject) Instantiate(Resources.Load("Prefabs/HookGun"), hand.position, hand.rotation, transform);
    }

    // Update is called once per frame
    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * viewRotationSpeed;
        mouseY += Input.GetAxis("Mouse Y") * viewRotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        movement = moveVertical * Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up) + 
                            moveHorizontal * mainCamera.transform.right;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

        if (isGrounded)
        {
            Vector3 vDesired = movementSpeed * movement.normalized;
            float k = (1 / Time.deltaTime) * 0.4f;
            Vector3 f = k * (vDesired - rb.velocity);
            //Prevent unrealistic forces by clamping to range
            f = Mathf.Clamp(f.magnitude, 0, 250.0f) * f.normalized;
            rb.AddForce(f, ForceMode.Acceleration);
        }
        view.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
        SmoothCameraMovement();
    }

    void SmoothCameraMovement()
    {
        Vector3 camPosition = mainCamera.transform.position;
        Quaternion camRotation = mainCamera.transform.rotation;
        mainCamera.transform.position = Vector3.Lerp(camPosition, camTarget.transform.position, cameraRigidness * Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Lerp(camRotation, camTarget.transform.rotation, cameraRigidness * Time.deltaTime);
    }

}
