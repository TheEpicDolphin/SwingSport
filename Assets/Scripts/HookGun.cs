﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    Rigidbody playerRb;

    bool isGrappled = false;

    const float maxRopeLength = 150.0f;

    float hookLaunchForce = 200.0f;

    float hookZoomRateMultiplier = 100.0f;

    public CameraWobbleDelegate camWobbleDelegate;

    public HookGunCursor cursor;

    private LineRenderer ropeRenderer;

    private VerletRope verletRope;

    public Material ropeMaterial;

    private void Awake()
    {
        state = HookState.Retracted;
        hookSlot = transform.Find("HookSlot");
        playerRb = GetComponentInParent<Rigidbody>();

        ropeMaterial = new Material(Shader.Find("Unlit/Color"));
        ropeMaterial.color = Color.red;

        ropeRenderer = gameObject.AddComponent<LineRenderer>();
        ropeRenderer.material = ropeMaterial;
        ropeRenderer.widthMultiplier = 0.05f;
        ropeRenderer.enabled = false;

        cursor = new HookGunCursor();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckHookableAndAdjustCursor();
        switch (state)
        {
            case HookState.Retracted:
                if (Input.GetMouseButtonDown(0))
                {
                    /* The hookGun has been fired */
                    Vector3 targetPos;
                    Ray camRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                    /* Prevent raycast from hitting something in front of camera but behind gun */
                    float startT = Vector3.Dot(transform.position - camRay.origin, camRay.direction);
                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(camRay.GetPoint(startT), camRay.direction), out hit, 20.0f))
                    {
                        targetPos = hit.point;
                    }
                    else
                    {
                        targetPos = camRay.GetPoint(maxRopeLength);
                    }
                    Vector3 launchDir = (targetPos - hookSlot.position).normalized;
                    GameObject hookGO = (GameObject)Instantiate(Resources.Load("Prefabs/Hook"), hookSlot.position, hookSlot.rotation);
                    hook = hookGO.GetComponent<Hook>();
                    Rigidbody hookRb = hook.GetComponent<Rigidbody>();
                    //Add launching force to hook
                    hookRb.AddForce(hookLaunchForce * launchDir, ForceMode.Impulse);
                    state = HookState.Launching;
                    StartCoroutine(LaunchHookCoroutine());
                }
                break;
            case HookState.Launching:
                /* The hook is currently being launched (controlled by coroutine) */

                break;
            case HookState.Attached:

                if (!Input.GetMouseButton(0))
                {
                    /* The hook has been detached */
                    isGrappled = false;
                    hook.transform.parent = null;
                    Destroy(verletRope);
                    state = HookState.Retracting;
                    StartCoroutine(RetractHookCoroutine());

                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    /* Reduce rope length so that player zooms to hook point*/
                    camWobbleDelegate?.Invoke(Mathf.Max(Mathf.Min(playerRb.velocity.magnitude, 90.0f) - 30.0f, 0.0f) / 60.0f);
                    verletRope.DecreaseRestLength(hookZoomRateMultiplier * Time.fixedDeltaTime);
                }
                else if (Input.GetKey("q"))
                {
                    /* Increase rope length so that player gets farther from hook point */
                    verletRope.IncreaseRestLength(10.0f * Time.fixedDeltaTime);
                }
                break;
            case HookState.Retracting:
                break;
        }
    }

    public void CheckHookableAndAdjustCursor()
    {
        bool cursorRed = false;
        Ray camRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        /* Prevent raycast from hitting something in front of camera but behind gun */
        float startT = Vector3.Dot(transform.position - camRay.origin, camRay.direction);
        RaycastHit hit;
        if (Physics.Raycast(new Ray(camRay.GetPoint(startT), camRay.direction), out hit, maxRopeLength))
        {
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Hookable")
                {
                    cursorRed = true;
                }
            }
        }

        if (cursorRed)
        {
            cursor.SetCursorColor(Color.red);
        } else
        {
            cursor.SetCursorColor(Color.white);
        }
    }

    IEnumerator LaunchHookCoroutine()
    {
        ropeRenderer.enabled = true;
        ropeRenderer.positionCount = 2;

        while (Vector3.Distance(transform.position, hook.transform.position) < maxRopeLength)
        {
            Collider[] colliders = new Collider[1];
            if (Physics.OverlapSphereNonAlloc(hook.transform.position, 0.1f, colliders) > 0)
            {
                ropeRenderer.enabled = false;
                if (colliders[0].tag == "Hookable")
                {
                    hook.GetComponent<Rigidbody>().isKinematic = true;
                    hook.transform.parent = colliders[0].transform;

                    isGrappled = true;
                    GameObject verletRopeGO = new GameObject();
                    verletRope = verletRopeGO.AddComponent<VerletRope>();
                    verletRope.BuildRope(this.gameObject, hook.gameObject, 6, maxRopeLength, ropeMaterial);
                    state = HookState.Attached;
                }
                else
                {
                    state = HookState.Retracting;
                    StartCoroutine(RetractHookCoroutine());
                }
                yield break;
            }
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPositions(new Vector3[] { transform.position, hook.transform.position });
            yield return null;
        }
        state = HookState.Retracting;
        ropeRenderer.enabled = false;
        StartCoroutine(RetractHookCoroutine());
        yield return null;
    }

    IEnumerator RetractHookCoroutine()
    {
        ropeRenderer.enabled = true;
        ropeRenderer.positionCount = 2;
        float t = 0.0f;
        float animDuration = 0.3f;
        Vector3 initialHookPos = hook.transform.position;
        while (t < animDuration)
        {
            hook.transform.position = Vector3.Lerp(initialHookPos, hookSlot.position, t / animDuration);
            t += Time.deltaTime;
            
            ropeRenderer.SetPositions(new Vector3[] { transform.position, hook.transform.position });
            yield return null;
        }
        hook.transform.position = hookSlot.position;
        Destroy(hook.gameObject);
        ropeRenderer.enabled = false;
        state = HookState.Retracted;
        yield return null;
    }

}
