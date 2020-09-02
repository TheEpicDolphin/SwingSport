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

    LineRenderer ropeRenderer;
    float maxRopeLength = 20.0f;
    Rigidbody playerRb;
    bool isGrappled = false;
    bool retract = false;
    bool release = false;

    private void Awake()
    {
        state = HookState.Retracted;
        hookSlot = transform.Find("HookSlot");
        playerRb = GetComponentInParent<Rigidbody>();

        ropeRenderer = gameObject.AddComponent<LineRenderer>();
        ropeRenderer.material = new Material(Shader.Find("Unlit/Color"));
        ropeRenderer.material.color = Color.red;
        ropeRenderer.widthMultiplier = 0.05f;
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
                    //Add launching force
                    hookRb.AddForce(30.0f * launchDir, ForceMode.Impulse);
                    state = HookState.Launching;
                    StartCoroutine(LaunchHookCoroutine());
                }
                break;
            case HookState.Launching:
                break;
            case HookState.Attached:
                if (!Input.GetMouseButton(0))
                {
                    //playerRb.isKinematic = false;
                    isGrappled = false;
                    hook.transform.parent = null;
                    state = HookState.Retracting;
                    StartCoroutine(RetractHookCoroutine());
                }
                else if (Input.GetKey(KeyCode.CapsLock))
                {
                    retract = true;
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    release = true;
                }
                break;
            case HookState.Retracting:
                break;
        }

        DrawRope();
    }

    IEnumerator LaunchHookCoroutine()
    {
        while (Vector3.Distance(transform.position, hook.transform.position) < maxRopeLength)
        {
            Collider[] colliders = new Collider[1];
            if (Physics.OverlapSphereNonAlloc(hook.transform.position, 0.1f, colliders, 1 << 10) > 0)
            {
                hook.GetComponent<Rigidbody>().isKinematic = true;
                hook.transform.parent = colliders[0].transform;

                //playerRb.isKinematic = true;
                isGrappled = true;
                state = HookState.Attached;
                yield break;
            }
            yield return null;
        }
        state = HookState.Retracting;
        StartCoroutine(RetractHookCoroutine());
        yield return null;
    }

    IEnumerator RetractHookCoroutine()
    {
        float t = 0.0f;
        float animDuration = 0.3f;
        Vector3 initialHookPos = hook.transform.position;
        while (t < animDuration)
        {
            hook.transform.position = Vector3.Lerp(initialHookPos, hookSlot.position, t / animDuration);
            t += Time.deltaTime;
            yield return null;
        }
        hook.transform.position = hookSlot.position;
        Destroy(hook.gameObject);
        state = HookState.Retracted;
        yield return null;
    }

    private void FixedUpdate()
    {
        
        if (isGrappled)
        {
            //Maybe add force to this guy?
            Rigidbody connectedTo = hook.GetComponentInParent<Rigidbody>();

            Vector3 toHookVector = hook.transform.position - playerRb.transform.position;
            Vector3 directionToHook = toHookVector.normalized;
            float distanceToHook = directionToHook.magnitude;

            Vector3 velTowardsHook = Vector3.Project(playerRb.velocity, directionToHook);
            playerRb.velocity = playerRb.velocity - velTowardsHook;
            
            if (retract)
            {
                playerRb.velocity += 8.0f * directionToHook;
                retract = false;
            }
            else if (release)
            {
                playerRb.velocity += -8.0f * directionToHook;
                release = false;
            }

            Vector3 customGravity = Physics.gravity * playerRb.mass;
            Vector3 ropeTension = -Vector3.Project(customGravity, directionToHook);
            playerRb.AddForce(ropeTension);
            //Debug.Log(Vector3.Dot(playerRb.velocity, Vector3.down));
            
        }
        
    }

    void DrawRope()
    {
        if (hook)
        {
            List<Vector3> ropeVerts = new List<Vector3>();
            ropeVerts.Add(transform.position);
            ropeVerts.Add(hook.transform.position);
            ropeRenderer.positionCount = ropeVerts.Count;
            ropeRenderer.SetPositions(ropeVerts.ToArray());
            ropeRenderer.enabled = true;
        }
        else
        {
            ropeRenderer.enabled = false;
        }
    }
}
