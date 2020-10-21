using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAttachment : RopeNode
{
    public Transform attachmentTransform;

    public Rigidbody rb;

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

    public static void ApplyTension(RopeAttachment ra1, RopeAttachment ra2)
    {
        float Ck = 0.1f;
        float Cd = 0.1f;

        Vector3 x = ra2.attachmentTransform.position - ra1.attachmentTransform.position;
        Vector3 direction = x.normalized;
        Vector3 x0 = (ra2.ropeLocation - ra1.ropeLocation) * direction;
        if (x.sqrMagnitude > x0.sqrMagnitude)
        {
            float m_red = 1.0f / (1 / ra1.Mass() + 1 / ra2.Mass());
            Vector3 v_rel = Vector3.Project(ra1.Velocity() - ra2.Velocity(), direction);
            float dt = Time.fixedDeltaTime;
            float k = m_red * Ck / (dt * dt);
            float b = m_red * Cd / dt;
            Vector3 f1 = k * (x - x0) - b * v_rel;
            Vector3 f2 = -k * (x - x0) + b * v_rel;
            ra1.AddForce(f1, ForceMode.Force);
            ra2.AddForce(f2, ForceMode.Force);
        }
    }

    public override void ApplyConstraint(VerletParticle vp)
    {
        float constraintLength = vp.ropeLocation - ropeLocation;
        Vector3 d1 = vp.transform.position - transform.position;
        float d2 = d1.magnitude;
        float d3 = (d2 - constraintLength) / d2;
        vp.transform.position += -1.0f * d1 * d3;
    }

    public override void ApplyConstraint(RopeAttachment ra)
    {
        //Do nothing. We handle forces in ApplyTension
    }

    public void MoveAlongRope(float dt)
    {
        rope.ChangeRopeLocation(this, dt);
    }

    private void OnDestroy()
    {
        rope.Detach(this);
    }
}
