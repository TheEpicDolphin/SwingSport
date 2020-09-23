using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: will treat ball as intelligent homing missile
// TODO: make it adhere to IItem interface
public class MagnetoGlove : MonoBehaviour
{
    public float shrunkBallRadius = 0.5f;

    Transform ballTarget;

    /* Holds the ball in place when it is close enough to hand */
    FixedJoint ballHolder;

    Rigidbody handRb;

    float maxRange = 40.0f;
    float constantRange = 0.5f;
    float magneticCoeff;
    float breakForce = 1000.0f;
    /* This drag is performed against the ball's velocity component that 
     * is perpendicular to the direction from the glove to the ball*/
    float magneticBallDrag = 10.0f;

    bool equipped = true;

    private void Awake()
    {
        magneticCoeff = breakForce * constantRange * constantRange;

        //GameObject ballTargetGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Destroy(ballTargetGO.GetComponent<Collider>());
        GameObject ballTargetGO = new GameObject();
        ballTarget = ballTargetGO.transform;
        ballTarget.parent = transform;
        ballTarget.position = transform.position + shrunkBallRadius * transform.forward;
        ballTarget.rotation = Quaternion.identity;

        handRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // TODO: check if ball is in player's inventory.
        if(equipped && PlayerInputManager.Instance.leftCTRL && !ballHolder)
        {
            Collider[] colliders = new Collider[1];
            Physics.OverlapSphereNonAlloc(ballTarget.position, maxRange, colliders, 1 << 10);
            if (colliders[0])
            {
                Rigidbody ballRb = colliders[0].GetComponent<Rigidbody>();

                float ballToTargetDistance = Vector3.Distance(ballTarget.position, ballRb.transform.position);
                if (ballToTargetDistance < 0.25f)
                {
                    ballRb.isKinematic = true;
                    ballRb.MovePosition(ballTarget.position);
                    ballRb.isKinematic = false;

                    ballHolder = gameObject.AddComponent<FixedJoint>();
                    ballHolder.connectedBody = ballRb;
                    ballHolder.breakForce = breakForce;
                }
                else
                {
                    /* The ball only feels a force when within maxRange distance
                    * of glove */
                    Vector3 r = ballRb.transform.position - ballTarget.position;
                    /* If ball is within constantRange of glove, we do not increase
                     * magnetic field any further*/
                    float rMag = Mathf.Max(r.magnitude, constantRange);
                    Vector3 fMagnetic = (magneticCoeff / (rMag * rMag)) * r.normalized;
                    /* This will help the ball go directly to the glove without oscillating
                     * as much*/
                    Vector3 perpBallVel = Vector3.ProjectOnPlane(ballRb.velocity, r);
                    Vector3 ballDragForce = -magneticBallDrag * perpBallVel;
                    ballRb.AddForce(-fMagnetic + ballDragForce);
                    handRb.AddForce(fMagnetic);
                }
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(ballTarget.gameObject);
    }

}
