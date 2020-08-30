using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Is listener to HookGun
public class Hook : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Attached())
        {
            gameObject.AddComponent<FixedJoint>().connectedBody = other.GetComponent<Rigidbody>();
        }
    }

    public bool Attached()
    {
        return GetComponent<FixedJoint>() != null;
    }

    public void Detach()
    {
        FixedJoint fixedJoint = GetComponent<FixedJoint>();
        if (fixedJoint)
        {
            Debug.Log("DESTROYED");
            Destroy(fixedJoint);
        }
    }
}
