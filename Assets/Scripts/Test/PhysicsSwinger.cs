using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSwinger : MonoBehaviour
{
    public Transform physicsAnchor;
    const float maxRopeLength = 150.0f;
    VerletRope verletRope;
    // Start is called before the first frame update
    void Start()
    {
        Material ropeMaterial = new Material(Shader.Find("Unlit/Color"));

        GameObject gun = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Rigidbody gunRb = gun.AddComponent<Rigidbody>();
        gunRb.isKinematic = true;
        Destroy(gun.GetComponent<Collider>());
        gun.transform.position = transform.position + 0.25f * transform.forward + 0.25f * transform.right;
        gun.transform.rotation = transform.rotation;
        gun.transform.parent = transform;
        VerletRope verletRope = gun.AddComponent<VerletRope>();
        //VerletRope verletRope = gameObject.AddComponent<VerletRope>();
        verletRope.BuildRope(physicsAnchor, 6, maxRopeLength, ropeMaterial);
        verletRope.Spring = 5000.0f;
        verletRope.Damper = 1000.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
