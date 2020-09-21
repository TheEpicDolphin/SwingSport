using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetoGlove : MonoBehaviour
{
    FixedJoint ballHolder;

    float maxRange = 20.0f;
    float magneticCoeff = 10.0f;

    /* This drag is performed against the ball's velocity component that 
     * is perpendicular to the direction from the glove to the ball*/
    float magneticBallDrag = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        ballHolder = GetComponent<FixedJoint>();
        ballHolder.breakForce = 200.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(equipped && PlayerInputManager.Instance.leftMouse && !ballHolder.connectedBody)
        {
            Collider[] colliders = new Collider[1];
            Physics.OverlapSphereNonAlloc(transform.position, maxRange, colliders, 1 << 10);
            if (colliders[0])
            {
                /* The ball only feels a force when within maxRange distance
                 * of glove */
                Rigidbody ballRb = colliders[0].GetComponent<Rigidbody>();
                Vector3 r = ballRb.transform.position - transform.position;
                /* If ball is within 0.5f of glove, we do not increase
                 * magnetic field any further*/
                float rMag = Mathf.Max(r.magnitude, 0.5f);
                Vector3 fMagnetic = r * magneticCoeff / (rMag * rMag);
                /* This will help the ball go directly to the glove without oscillating
                 * as much*/
                Vector3 perpBallVel = Vector3.ProjectOnPlane(ballRb.velocity, r);
                Vector3 ballDragForce = -magneticBallDrag * perpBallVel;
                ballRb.AddForce(fMagnetic + ballDragForce);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /* Handle when ball is close enough to glove to be grabbed */
        if (equipped && PlayerInputManager.Instance.leftMouse && 
            other.tag == "ball")
        {
            Rigidbody ballRb = other.GetComponent<Rigidbody>();
            if (ballRb)
            {
                ballRb.isKinematic = true;
                ballRb.MovePosition(transform.position);
                ballRb.isKinematic = false;
                ballHolder.connectedBody = ballRb;
            }
        }
    }

}
