using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Transform view;
    Transform camTarget;
    Transform hand;
    Transform character;
    public Transform mainCamera;

    /* mouse position on screen */ 
    float mouseX, mouseY;

    /* mouse sensitivity */
    float mouseSensitivity = 5.0f;

    /* how fast player can move */
    float movementSpeed = 8.0f;

    /* movement vector of the player*/
    Vector3 movement = Vector3.zero;

    /* determines whether the player is touching the ground */
    public bool isGrounded = false;

    /* if true, player is rocketing up */
    public bool rocketUp = false;

    /* determines whether the player will look in the direction of the camera or not */
    public bool isPlayerLockedToCamera = true;

    public float bDrag = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        /* removes mouse cursor and locks cursor to center of the screen */
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        view = transform.Find("View");
        character = transform.Find("Character");
        hand = character.Find("Hand");
        camTarget = view.GetChild(0);

        //Instantiate hook gun in player's hand
        GameObject hookGunGO = (GameObject) Instantiate(Resources.Load("Prefabs/HookGun"), hand.position, hand.rotation, hand);
        HookGun hookGun = hookGunGO.GetComponent<HookGun>();
        hookGun.camWobbleDelegate = mainCamera.GetComponent<CameraController>().AddWobble;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -80, 80);

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        movement = moveVertical * Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized + 
                            moveHorizontal * mainCamera.transform.right;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            /* jump */
            rb.AddForce(300.0f * Vector3.up, ForceMode.Impulse);
        }

        rocketUp = Input.GetKey(KeyCode.Space) && !isGrounded;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position + 0.05f * Vector3.up, Vector3.down, 0.1f);

        if (isGrounded)
        {
            /* Player is on the ground */
            Vector3 vDesired = movementSpeed * movement.normalized;
            float k = (1 / Time.deltaTime) * 0.4f;
            Vector3 f = k * (vDesired - rb.velocity);
            rb.AddForce(f, ForceMode.Acceleration);
        }
        else
        {
            if (rocketUp)
            {
                rb.AddForce(-1.10f * Physics.gravity, ForceMode.Acceleration);
                rocketUp = false;
            }
            /* Player is in the air. Allow jetpack-like movement */
            rb.AddForce(10.0f * movement.normalized, ForceMode.Acceleration);

            /* Add drag to player */
            rb.AddForce(-bDrag * rb.velocity);
        }

        if (isPlayerLockedToCamera)
        {
            character.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
            view.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
        }
        else
        {
            view.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
        }
    }
}
