using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make it adhere to IItem interface
public class MagnetoGlove : MonoBehaviour
{
    public float ballRadius = 2.0f;

    Transform ballTarget;

    /* Holds the ball in place when it is close enough to hand */
    FixedJoint ballHolder;

    float maxRange = 20.0f;
    float magneticCoeff = 30.0f;
    /* This drag is performed against the ball's velocity component that 
     * is perpendicular to the direction from the glove to the ball*/
    float magneticBallDrag = 15.0f;

    bool equipped = true;

    private void Awake()
    {
        GameObject ballTargetGO = new GameObject();
        ballTarget = ballTargetGO.transform;
        ballTarget.parent = transform;
        ballTarget.position = transform.position + ballRadius * transform.forward;
        ballTarget.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (ballHolder && PlayerInputManager.Instance.spacebar)
        {
            ballHolder.connectedBody.AddForce(21.0f * Vector3.down);
        }

        // TODO: check if ball is in player's inventory.
        if(equipped && PlayerInputManager.Instance.leftMouse && !ballHolder)
        {
            Collider[] colliders = new Collider[1];
            Physics.OverlapSphereNonAlloc(ballTarget.position, maxRange, colliders, 1 << 10);
            if (colliders[0])
            {
                Rigidbody ballRb = colliders[0].GetComponent<Rigidbody>();
                Debug.Log(Vector3.Distance(ballTarget.position, ballRb.transform.position));
                if (Vector3.Distance(ballTarget.position, ballRb.transform.position) < 0.1f)
                {
                    ballRb.isKinematic = true;
                    ballRb.MovePosition(ballTarget.position);
                    ballRb.isKinematic = false;

                    ballHolder = gameObject.AddComponent<FixedJoint>();
                    ballHolder.connectedBody = ballRb;
                    ballHolder.breakForce = 20.0f;
                }
                else
                {
                    /* The ball only feels a force when within maxRange distance
                    * of glove */
                    Vector3 r = ballRb.transform.position - ballTarget.position;
                    /* If ball is within 0.5f of glove, we do not increase
                     * magnetic field any further*/
                    float rMag = Mathf.Max(r.magnitude, 0.5f);
                    Vector3 fMagnetic = r * magneticCoeff / (rMag * rMag);
                    /* This will help the ball go directly to the glove without oscillating
                     * as much*/
                    Vector3 perpBallVel = Vector3.ProjectOnPlane(ballRb.velocity, r);
                    Vector3 ballDragForce = -magneticBallDrag * perpBallVel;
                    ballRb.AddForce(-fMagnetic + ballDragForce);
                }

                
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(ballTarget.gameObject);
    }

}
