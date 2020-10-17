using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAttachment : RopeNode
{
    public Rigidbody rb;

    public Transform attachmentTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        if (rb)
        {
            rb.AddForceAtPosition(force, attachmentTransform.position, mode);
        }
    }

    float Mass()
    {
        return rb != null ? rb.mass : Mathf.Infinity;
    }

    Vector3 Velocity()
    {
        return rb != null ? rb.velocity : Vector3.zero;
    }

    public static void ApplyConstraint(RopeAttachment ra1, RopeAttachment ra2)
    {
        float Ck = 0.1f;
        float Cd = 0.1f;

        Vector3 x = ra2.attachmentTransform.position - ra1.attachmentTransform.position;
        Vector3 forceDirection = x.normalized;

        float m_red = 1.0f / (1 / ra1.Mass() + 1 / ra2.Mass());
        Vector3 v_rel = Vector3.Project(ra2.Velocity() - ra1.Velocity(), forceDirection);
        float dt = Time.fixedDeltaTime;
        Vector3 f = (-(m_red * Ck / (dt * dt)) * x - (m_red * Cd / dt) * v_rel);
        ra1.AddForce(f, ForceMode.Force);
        ra2.AddForce(-f, ForceMode.Force);
    }

}
