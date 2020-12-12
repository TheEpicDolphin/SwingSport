using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrapplingHookLauncher
{
    void DidHitCollider(Collider collider);

    void DidFinishRetracting();
}

public class GrapplingHook : MonoBehaviour
{
    Rigidbody rb;
    bool isLaunching;
    public IGrapplingHookLauncher launcher;
    MonoBehaviour activeMonoBehaviour;
    //GrapplingHookMonoBehaviour activeMonoBehaviour;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.AddComponent<Launching>();
        gameObject.AddComponent<Retracting>();
        isLaunching = false;
    }

    private void FixedUpdate()
    {
        if (isLaunching)
        {
            Collider[] colliders = new Collider[1];
            LayerMask obstacleMask = LayerMask.GetMask("Obstacle");
            LayerMask ballLayerMask = LayerMask.GetMask("Ball");
            if (Physics.OverlapSphereNonAlloc(transform.position, 0.15f, colliders, obstacleMask | ballLayerMask) > 0)
            {
                if (colliders[0].tag == "Hookable" || colliders[0].tag == "BounceBall")
                {
                    launcher.DidHitCollider(colliders[0]);
                    isLaunching = false;
                }
            }
        }
    }

    public void Launch(Vector3 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
        isLaunching = true;
        SetActiveMonoBehaviour<Launching>();
    }

    public void Attach(Transform trans)
    {
        rb.isKinematic = true;
        transform.parent = trans;
    }

    public void Retract()
    {
        rb.isKinematic = false;
        SetActiveMonoBehaviour<Retracting>();
    }

    private void SetActiveMonoBehaviour<T>() where T : MonoBehaviour
    {
        if (activeMonoBehaviour)
        {
            activeMonoBehaviour.enabled = false;
        }
        activeMonoBehaviour = gameObject.GetComponent<T>();
        activeMonoBehaviour.enabled = true;
    }
}
