using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetoGlove : MonoBehaviour
{
    float maxRange = 20.0f;
    float magneticCoeff = 10.0f;

    /* This drag is performed against the ball's velocity component that 
     * is perpendicular to the direction from the glove to the ball*/
    float magneticBallDrag = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(equipped && PlayerInputManager.Instance.leftMouse)
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
}
