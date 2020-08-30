using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HookGun : MonoBehaviour
{
    enum HookState
    {
        Retracted,
        Launching,
        Attached,
        Retracting
    }

    HookState state;
    Transform hookSlot;
    Hook hook;
    Rope rope;
    FixedJoint fixedJoint;
    float maxRopeLength = 20.0f;

    private void Awake()
    {
        state = HookState.Retracted;
        hookSlot = transform.Find("HookSlot");
        rope = GetComponent<Rope>();
        fixedJoint = GetComponent<FixedJoint>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case HookState.Retracted:
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 targetPos;
                    Ray camRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(camRay, out hit, 20.0f, 1 << 10))
                    {
                        targetPos = hit.point;
                    }
                    else
                    {
                        targetPos = camRay.GetPoint(20.0f);
                    }
                    Vector3 launchDir = (targetPos - hookSlot.position).normalized;
                    GameObject hookGO = (GameObject)Instantiate(Resources.Load("Prefabs/Hook"), hookSlot.position, hookSlot.rotation);
                    hook = hookGO.GetComponent<Hook>();
                    Rigidbody hookRb = hook.GetComponent<Rigidbody>();
                    rope.load = hookRb;
                    //rope.tautLength = 20.0f;
                    //Add launching force
                    hookRb.AddForce(10.0f * launchDir, ForceMode.Impulse);
                    state = HookState.Launching;
                    //Action hookAttachedAction = new Action(HookAttached);
                    //Action hookMissedAction = new Action(HookMissed);
                    //hook.Launch(launchDir, hookAttachedAction, hookMissedAction);
                }
                break;
            case HookState.Launching:
                if(rope.CurrentLength() > maxRopeLength)
                {
                    //Player missed, rope didnt hit anything
                    state = HookState.Retracting;
                }
                else if (hook.Attached())
                {
                    state = HookState.Attached;
                }
                else
                {
                    rope.Release();
                }
                break;
            case HookState.Attached:
                if (!Input.GetMouseButton(0))
                {
                    hook.Detach();
                    state = HookState.Retracting;
                }
                else if (Input.GetKey(KeyCode.CapsLock))
                {
                    rope.PullIn();
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    rope.Release();
                }
                break;
            case HookState.Retracting:
                rope.PullIn();
                /*
                if ()
                {
                    state = HookState.Retracted;
                }
                else
                {
                    rope.PullIn();
                }
                */
                break;
        }
    }

    /*
    void HookAttached()
    {
        rope.tautLength = Vector3.Distance(transform.position, rope.load.position);
        state = HookState.Attached;
    }

    void HookMissed()
    {
        state = HookState.Retracting;
        Action hookRetractedAction = new Action(HookRetracted);
        hook.StartRetracting(hookSlot, hookRetractedAction);
    }

    void HookRetracted()
    {
        state = HookState.Retracted;
    }
    */
}
