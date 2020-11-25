using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringState : GrappleGunState
{
    
    public FiringState(GrappleGunStateMachine grapplingGunSM, GrappleGun grappleGun) : base(grapplingGunSM, grappleGun)
    {

    }

    public override void OnEnter()
    {
        grappleGun.hook.isKinematic = false;
        grappleGun.hook.AddForce(grappleGun.hookLaunchForce * launchDir, ForceMode.Impulse);
        //grapplingGun.animator.CrossFade("Firing", 0.1f);
        grappleGun.fakeRopeRenderer.enabled = true;
    }

    public override void FixedUpdateStep()
    {
        if (Vector3.Distance(grappleGun.transform.position, grappleGun.hook.transform.position) > grappleGun.maxRopeLength)
        {
            // Put this in the OnEnter for the next state
            grappleGun.rope = Rope.CreateTautRope(grappleGun.transform.position, grappleGun.hook.transform.position);
            grappleGun.rope.Attach(hangerRA);
            grappleGun.rope.Attach(sphereRA);
            grappleGunSM.TransitionToState<ExtendedState>();
            return;
        }

        Collider[] colliders = new Collider[1];
        LayerMask obstacleMask = LayerMask.GetMask("Obstacle");
        LayerMask ballLayerMask = LayerMask.GetMask("Ball");
        if (Physics.OverlapSphereNonAlloc(grappleGun.hook.transform.position, 0.15f, colliders, obstacleMask | ballLayerMask) > 0)
        {
            if (colliders[0].tag == "Hookable" || colliders[0].tag == "BounceBall")
            {
                grappleGun.hook.isKinematic = true;
                grappleGun.hook.transform.parent = colliders[0].transform;
            }
            // Put this in the OnEnter for the next state
            grappleGun.rope = Rope.CreateTautRope(grappleGun.transform.position, grappleGun.hook.transform.position);
            grappleGun.rope.Attach(hangerRA);
            grappleGun.rope.Attach(sphereRA);
            grappleGunSM.TransitionToState<ExtendedState>();
        }
    }

    public override void UpdateStep()
    {
        
    }

    public override void OnExit()
    {
        grappleGun.fakeRopeRenderer.enabled = false;
    }
}
