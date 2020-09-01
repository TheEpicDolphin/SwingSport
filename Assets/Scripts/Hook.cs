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
}
