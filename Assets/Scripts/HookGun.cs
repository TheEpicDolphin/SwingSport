using System.Collections;
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

    LineRenderer ropeRenderer;

    Rigidbody playerRb;

    bool isGrappled = false;

    bool retract = false;

    bool release = false;

    float restRopeLength = 20.0f;

    const float maxRopeLength = 150.0f;

    float hookLaunchForce = 200.0f;

    float hookZoomRateMultiplier = 100.0f;

    public CameraWobbleDelegate camWobbleDelegate;

    public HookGunCursor cursor;

    private VerletRope verletRope;

    private void Awake()
    {
        state = HookState.Retracted;
        hookSlot = transform.Find("HookSlot");
        playerRb = GetComponentInParent<Rigidbody>();

        ropeRenderer = gameObject.AddComponent<LineRenderer>();
        ropeRenderer.material = new Material(Shader.Find("Unlit/Color"));
        ropeRenderer.material.color = Color.red;
        ropeRenderer.widthMultiplier = 0.05f;

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
                    //Destroy(verletRope);
                    state = HookState.Retracting;
                    StartCoroutine(RetractHookCoroutine());

                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    camWobbleDelegate(Mathf.Max(Mathf.Min(playerRb.velocity.magnitude, 90.0f) - 30.0f, 0.0f) / 60.0f);
                    /* Hook will be retracted */
                    retract = true;
                }
                else if (Input.GetKey("q"))
                {
                    /* Hook will be released */
                    release = true;

                }
                break;
            case HookState.Retracting:
                
                break;
        }

        DrawRope();
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
        //TODO: use fake rope here and animate it moving to the desired position

        while (Vector3.Distance(transform.position, hook.transform.position) < maxRopeLength)
        {
            Collider[] colliders = new Collider[1];
            if (Physics.OverlapSphereNonAlloc(hook.transform.position, 0.1f, colliders) > 0)
            {
                if(colliders[0].tag == "Hookable")
                {
                    hook.GetComponent<Rigidbody>().isKinematic = true;
                    hook.transform.parent = colliders[0].transform;

                    isGrappled = true;
                    restRopeLength = Vector3.Distance(playerRb.transform.position, hook.transform.position);
                    //verletRope = Instantiate();
                    //verletRope.BuildRope(this.gameObject, hook.gameObject, 4);
                    state = HookState.Attached;
                }
                else
                {
                    state = HookState.Retracting;
                    StartCoroutine(RetractHookCoroutine());
                }
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
        //TODO: use fake rope here and animate it to retract

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

            if (retract)
            {
                /* Reduce rope length so that player zooms to hook point*/
                Debug.Log(hookZoomRateMultiplier * Time.fixedDeltaTime);
                restRopeLength = Mathf.Max(restRopeLength - hookZoomRateMultiplier * Time.fixedDeltaTime, 0.0f);
                retract = false;
            }
            else if (release)
            {
                /* Increase rope length so that player gets farther from hook point */
                restRopeLength = Mathf.Min(restRopeLength + 10.0f * Time.fixedDeltaTime, maxRopeLength);
                release = false;
            }
            
            float k = 500.0f;
            /* Critically damped */
            float b = Mathf.Sqrt(4 * playerRb.mass * k);
            /* Treating rope like a spring */
            Vector3 fSpring = -k * (restRopeLength * directionToHook - toHookVector)
                                + b * (Vector3.zero - Vector3.Project(playerRb.velocity, directionToHook));
            playerRb.AddForce(fSpring);
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
