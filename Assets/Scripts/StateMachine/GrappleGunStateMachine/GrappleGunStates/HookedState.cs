using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookedState : GrappleGunState
{
    RopeAttachment grappleGunRA;
    Transform hookedTransform;

    public HookedState(GrappleGun grappleGun, Collider hookedCollider) : base(grappleGun)
    {
        hookedTransform = hookedCollider.transform;
        
    }

    public override void OnEnter()
    {
        grappleGun.AttachHookToTransform(hookedTransform);

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
            grappleGun.stateMachine.TransitionToState(new RetractingHookState(grappleGun));
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
