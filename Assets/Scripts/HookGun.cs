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

    Rigidbody hookGunRb;

    Hook hook;

    FixedJoint attachJoint;

    bool isGrappled = false;

    const float maxRopeLength = 150.0f;

    float hookLaunchForce = 200.0f;

    float hookZoomRateMultiplier = 20.0f;

    public HookGunCursor cursor;

    private LineRenderer ropeRenderer;

    private VerletRope verletRope;

    public Material ropeMaterial;

    private int mouseLaunchButton;

    public void setControls(int mouseLaunchButton)
    {
        this.mouseLaunchButton = mouseLaunchButton;
    }

    public void setColor(Color c)
    {
        ropeMaterial.color = c;
    }

    private void Awake()
    {
        state = HookState.Retracted;
        hookSlot = transform.Find("HookSlot");

        ropeMaterial = new Material(Shader.Find("Unlit/Color"));

        ropeRenderer = gameObject.GetComponent<LineRenderer>();
        if (!ropeRenderer)
        {
            ropeRenderer = gameObject.AddComponent<LineRenderer>();
            ropeRenderer.material = ropeMaterial;
            ropeRenderer.widthMultiplier = 0.05f;
            ropeRenderer.enabled = false;
        }

        cursor = new HookGunCursor();
        hookGunRb = GetComponent<Rigidbody>();
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
                if (Input.GetMouseButtonDown(mouseLaunchButton))
                {
                    /* The hookGun has been fired */
                    Vector3 targetPos;
                    Ray camRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                    /* Prevent raycast from hitting something in front of camera but behind gun */
                    float startT = Vector3.Dot(transform.position - camRay.origin, camRay.direction);
                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(camRay.GetPoint(startT), camRay.direction), out hit, maxRopeLength))
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
                // check to make sure that the hook hasn't been deleted (as is the case
                // when the player grabs a ball and pulls it within grabbing distance)
                if (hook == null)
                {
                    // if the hook was deleted, treat it as if the hook is retracted
                    /* The hook has been detached */
                    isGrappled = false;
                    Destroy(verletRope);
                    state = HookState.Retracted;
                }
                else if (!Input.GetMouseButton(mouseLaunchButton))
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
                if (hit.collider.tag == "Hookable" || hit.collider.tag == "BounceBall")
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
            LayerMask obstacleMask = LayerMask.GetMask("Obstacle");
            LayerMask ballLayerMask = LayerMask.GetMask("Ball");
            if (Physics.OverlapSphereNonAlloc(hook.transform.position, 0.1f, colliders, obstacleMask | ballLayerMask) > 0)
            {
                //ropeRenderer.enabled = false;
                if (colliders[0].tag == "Hookable" || colliders[0].tag == "BounceBall")
                {
                    hook.GetComponent<Rigidbody>().isKinematic = true;
                    hook.transform.parent = colliders[0].transform;

                    isGrappled = true;

                    verletRope = gameObject.AddComponent<VerletRope>();
                    verletRope.BuildRope(hook.transform, 6, maxRopeLength, ropeMaterial);
                    verletRope.Spring = 5000.0f;
                    verletRope.Damper = 1000.0f;
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
        //ropeRenderer.enabled = false;
        StartCoroutine(RetractHookCoroutine());
        yield return null;
    }

    IEnumerator RetractHookCoroutine()
    {
        //ropeRenderer.enabled = true;
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

    public void Equip(Transform parent, Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        hookGunRb.isKinematic = true;
        transform.parent = parent;
    }

}
