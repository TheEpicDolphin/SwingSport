﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make it adhere to IItem interface
public class MagnetoGlove : MonoBehaviour
{
    public float shrunkBallRadius = 0.5f;

    public Transform ballTarget;

    Renderer ballGrabbingRegionRenderer;

    /* This can vary as glove loses magnetic power */
    public float Strength { get; private set; } = 20.0f;

    public bool IsMagnetizing { get; private set; } = false;

    public bool visualizeGrabbingRegion = false;

    /* Holds the ball in place when it is close enough to hand */
    FixedJoint ballHolder;

    Rigidbody handRb;

    float constantRange = 1.0f;
    
    float breakForce = 1000.0f;

    bool equipped = true;

    SphereCollider sphereCollider;

    private void Awake()
    {
        GameObject ballTargetGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(ballTargetGO.GetComponent<Collider>());
        ballGrabbingRegionRenderer = ballTargetGO.GetComponent<Renderer>();

        ballTarget = ballTargetGO.transform;
        ballTarget.parent = transform;
        ballTarget.position = transform.position + shrunkBallRadius * transform.forward;
        ballTarget.rotation = Quaternion.identity;

        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = 0.5f;
        sphereCollider.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer("MagnetoGlove");
    }

    public void Equip(Transform parent, Vector3 position, Quaternion rotation, bool usePhysics)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.parent = parent;
        handRb = parent.GetComponent<Rigidbody>();
    }

    public void ApplyForceOnHand(Vector3 force, ForceMode forceMode)
    {
        handRb.AddForceAtPosition(force, transform.position, forceMode);
    }

    // Update is called once per frame
    void Update()
    {
        ballGrabbingRegionRenderer.enabled = visualizeGrabbingRegion;
        IsMagnetizing = PlayerInputManager.Instance.capsLock;
    }

    private void OnDestroy()
    {
        Destroy(ballTarget.gameObject);
    }

}
