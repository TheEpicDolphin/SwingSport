using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputManager : MonoBehaviour
{
    /*
    // Singleton Pattern
    private static PlayerInputManager instance;

    public static PlayerInputManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType(typeof(PlayerInputManager)) as PlayerInputManager;
                if (!instance)
                {
                    Debug.LogError("No active PlayerInputManager script ony any GameObject.");
                }
            }
            return instance;
        }
    }
    */

    public float horizontal;
    public float vertical;
    public float mouseXDelta;
    public float mouseYDelta;

    public bool spacebar;
    public bool spacebarDown;
    public bool capsLock;
    public bool Q;
    public bool QDown;

    public bool leftMouse;
    public bool leftMouseDown;

    public bool rightMouse;
    public bool rightMouseDown;

    // Start is called before the first frame update
    void Start()
    {
        /* removes mouse cursor and locks cursor to center of the screen */
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        mouseXDelta = Input.GetAxis("Mouse X");
        mouseYDelta = Input.GetAxis("Mouse Y");

        spacebar = Input.GetKey(KeyCode.Space);
        spacebarDown = Input.GetKeyDown(KeyCode.Space);
        capsLock = Input.GetKey(KeyCode.CapsLock);
        QDown = Input.GetKeyDown(KeyCode.Q);

        leftMouse = Input.GetMouseButton(0);
        leftMouseDown = Input.GetMouseButtonDown(0);

        rightMouse = Input.GetMouseButton(1);
        rightMouseDown = Input.GetMouseButtonDown(1);
    }
}
