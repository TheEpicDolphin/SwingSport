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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isLaunching = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(Vector3 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
        isLaunching = true;
    }

    public void StickToTransformAtPositionWithOrientation(Transform trans, Vector3 position, Quaternion orientation)
    {
        rb.isKinematic = true;
        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLaunching)
        {
            launcher.DidHitCollider(other);
        }
    }
}
