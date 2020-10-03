using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Make this a singleton
public class PlayerInputManager : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float mouseXDelta;
    public float mouseYDelta;

    public bool spacebar;
    public bool spacebarDown;
    public bool capsLock;
    public bool Q;
    public bool QDown;

    public UnityEvent leftMouseEvent = new UnityEvent();
    public UnityEvent leftMouseDownEvent = new UnityEvent();

    public UnityEvent rightMouseEvent = new UnityEvent();
    public UnityEvent rightMouseDownEvent = new UnityEvent();


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

        if (Input.GetMouseButton(0))
        {
            leftMouseEvent.Invoke();
        }
        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDownEvent.Invoke();
        }

        if (Input.GetMouseButton(1))
        {
            rightMouseEvent.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            rightMouseDownEvent.Invoke();
        }
    }
}
