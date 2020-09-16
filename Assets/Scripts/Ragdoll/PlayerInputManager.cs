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
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        mouseXDelta = Input.GetAxis("Mouse X");
        mouseYDelta = Input.GetAxis("Mouse Y");
    }
}
