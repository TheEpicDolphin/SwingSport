using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make this a singleton
public class PlayerInputManager : MonoBehaviour
{
    private static PlayerInputManager _instance;

    public static PlayerInputManager Instance { get { return _instance; } }

    public float horizontal;
    public float vertical;
    public float mouseXDelta;
    public float mouseYDelta;

    public bool spacebar;
    public bool spacebarDown;
    public bool leftCTRL;
    public bool leftMouse;
    public bool leftMouseDown;
    


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

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

        leftMouse = Input.GetMouseButton(0);
        leftMouseDown = Input.GetMouseButtonDown(0);

        leftCTRL = Input.GetKey(KeyCode.LeftControl);
    }
}
