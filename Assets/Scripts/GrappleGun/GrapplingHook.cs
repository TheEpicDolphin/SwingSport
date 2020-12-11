using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrapplingHookLauncher
{
    void DidHitCollider(Collider collider);
}

public class GrapplingHook : MonoBehaviour
{
    Rigidbody rb;
    bool isLaunching;
    public IGrapplingHookLauncher launcher;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
    }

    public void AttachToTransformAtPositionWithOrientation(Transform trans, Vector3 position, Quaternion orientation)
    {
        rb.isKinematic = true;
        transform.position = position;
    }
}
