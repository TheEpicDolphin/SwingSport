using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookedState : GrappleGunState
{
    RopeAttachment grappleGunRA;
    public HookedState(GrappleGunStateMachine grappleGunSM, GrappleGun grappleGun) : base(grappleGunSM, grappleGun)
    {
        
    }

    public override void OnEnter()
    {
        // TODO: make function in rope class that creates rope passing through several points

        grappleGunRA = grappleGun.gameObject.AddComponent<RopeAttachment>();
        grappleGun.rope = Rope.CreateInterpolatedRope();
        grappleGun.rope = Rope.CreateTautRope(grappleGun.transform.position, grappleGun.hook.transform.position);
        grappleGun.rope.Attach(hangerRA);
        grappleGun.rope.Attach(grappleGunRA);
    }

    public override void FixedUpdateStep()
    {
        
    }

    public override void UpdateStep()
    {
        if (!grappleGun.input.leftMouseDown)
        {
            grappleGunSM.TransitionToState<RetractingHookState>();
            return;
        }
        else if (Input.GetKey(KeyCode.CapsLock))
        {
            // Drawing in rope
            grappleGunRA.RemoveRopeAbove(Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            // Lengthening rope
            grappleGunRA.InsertRopeAbove(Time.deltaTime);
        }
        
    }

    public override void OnExit()
    {
        Destroy(grappleGunRA);
    }
}
