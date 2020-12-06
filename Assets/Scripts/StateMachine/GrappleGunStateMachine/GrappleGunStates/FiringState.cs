using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringState : GrappleGunState
{
    float firingTime;
    const float maxFiringTime = 2.0f;
    public FiringState(GrappleGunStateMachine grappleGunSM, GrappleGun grappleGun) : base(grappleGunSM, grappleGun)
    {

    }

    public override void OnEnter()
    {
        firingTime = 0.0f;
        grappleGun.ShootHook();
        //grapplingGun.animator.CrossFade("Firing", 0.1f);
        grappleGun.fakeRopeRenderer.enabled = true;
    }

    public override void FixedUpdateStep()
    {
        Collider[] colliders = new Collider[1];
        LayerMask obstacleMask = LayerMask.GetMask("Obstacle");
        LayerMask ballLayerMask = LayerMask.GetMask("Ball");
        if (Physics.OverlapSphereNonAlloc(grappleGun.hook.transform.position, 0.15f, colliders, obstacleMask | ballLayerMask) > 0)
        {
            if (colliders[0].tag == "Hookable" || colliders[0].tag == "BounceBall")
            {
                grappleGun.hook.isKinematic = true;
                grappleGun.hook.transform.parent = colliders[0].transform;
                grappleGunSM.TransitionToState<HookedState>();
                return;
            }
            else
            {
                grappleGunSM.TransitionToState<RetractingHookState>();
                return;
            }
            
        }
    }

    public override void UpdateStep()
    {
        firingTime += Time.deltaTime;
        if(firingTime > maxFiringTime)
        {
            grappleGunSM.TransitionToState<RetractingHookState>();
        }
    }

    public override void OnExit()
    {
        grappleGun.fakeRopeRenderer.enabled = false;
    }
}
