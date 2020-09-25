using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int numPlayers = 1;
    public float influnceRange = 40.0f;

    Rigidbody ballRb;
    SphereCollider ballCollider;

    public bool IsPossessed
    {
        get
        {
            return transform.parent != null;
        }
    }

    
    MagnetoGlove possessor;
    public MagnetoGlove Possessor
    {
        get
        {
            return possessor;
        }
        set
        {
            if(value)
            {
                ballCollider.enabled = false;
                ballRb.isKinematic = true;
                transform.parent = value.transform;
                possessor = value;
            }
            else
            {
                transform.parent = null;
                ballRb.isKinematic = false;
                ballCollider.enabled = true;
                possessor = null;
            }
        }
    }
    

    float maxSpeed = 35.0f;
    float kP = 5.0f;

    private void Awake()
    {
        ballRb = GetComponent<Rigidbody>();
        ballRb.isKinematic = false;
        ballCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (hasBall)
        {

            currentGrabTime -= Time.deltaTime;

            hasBallVisual.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.red, 1.0f - (currentGrabTime / maxGrabTime));

            if (Input.GetKeyDown(KeyCode.Q))
            {
                letGoOfBall(true);
            }
            else if (currentGrabTime < 0.0f)
            {
                letGoOfBall(false);
            }

        }
        */
    }

    private void FixedUpdate()
    {
        Collider[] colliders = new Collider[numPlayers];
        LayerMask magnetogloveLayerMask = LayerMask.GetMask("MagnetoGlove");
        int overlapCount = Physics.OverlapSphereNonAlloc(transform.position, influnceRange, colliders, magnetogloveLayerMask);
        for(int i = 0; i < overlapCount; i++)
        {
            Collider collider = colliders[i];
            MagnetoGlove magnetoGlove = collider.GetComponent<MagnetoGlove>();
            if (collider.transform != transform.parent && magnetoGlove.IsMagnetizing)
            {
                Vector3 r = magnetoGlove.ballTarget.position - transform.position;
                //Vector3 homingForce = InverseSquareForceLaw(magnetoGlove.Strength, r);
                //Vector3 homingForce = InverseForceLaw(magnetoGlove.Strength, r);
                //Vector3 homingForce = PControllerForce(magnetoGlove.ballTarget.position);
                Vector3 homingForce = PControllerWithPredictionForce(magnetoGlove.ballTarget.position, magnetoGlove.ballTargetVelocity);
                ballRb.AddForce(homingForce);
                magnetoGlove.ApplyForceOnHand(-homingForce, ForceMode.Force);
            }
        }
    }

    Vector3 InverseSquareForceLaw(float magCoeff, Vector3 r)
    {
        return (magCoeff / r.sqrMagnitude) * r.normalized;
    }

    Vector3 InverseForceLaw(float magCoeff, Vector3 r)
    {
        return (magCoeff / r.magnitude) * r.normalized;
    }

    Vector3 PControllerForce(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;
        Vector3 currentVelocity = ballRb.velocity;
        Vector3 desiredVelocity = maxSpeed * (targetPosition - currentPosition).normalized;
        Vector3 error = desiredVelocity - currentVelocity;
        return kP * error;
    }

    Vector3 PControllerWithPredictionForce(Vector3 targetPosition, Vector3 targetVelocity)
    {
        /*  
         *          b
         *  p(t) ________ p(t+dt)      
         *      |      /
         *      |     / 
         *      |    /
         *   a  |   /  c
         *      |  /
         *      | /
         *      |/
         *      p_b(t)  <-- ball
         *      
         *  a = p(t) - p_b(t)
         *  b = p(t + dt) - p(t) = v(t) * dt
         *  c = p(t + dt) - p_b(t) = v_b * dt = a + b
         *  ||c||^2 = ||a + b||^2
         *  dot(c, c) = dot(a, a) + dot(b, b) + 2 * dot(a, b)
         *  a = p_rel(t) = p(t) - p_b(t)
         *  
         *  ||v_b|| * dt^2 = dot(p_rel(t),p_rel(t)) + dot(v(t),v(t)) * dt^2 + 2 * dot(p_rel(t),v(t)) * dt
         *  0 = (dot(v(t),v(t)) - ||v_b||^2) * dt^2 + dot(p_rel(t),v(t)) * dt + dot(p_rel(t),p_rel(t))
         *  ...Solve for dt
         */

        //Vector3 vRel = targetVelocity - ballRb.velocity;
        Vector3 pRel = targetPosition - transform.position;

        float a = (Vector3.Dot(targetVelocity, targetVelocity) - maxSpeed * maxSpeed);
        float b = Vector3.Dot(pRel, targetVelocity);
        float c = Vector3.Dot(pRel, pRel);

        /* 0 = at^2 + bt + c */
        /* c >= 0 */

        if(Mathf.Abs(a) < 1e-4f)
        {
            if(Mathf.Abs(b) < 1e-4f)
            {
                return PControllerForce(targetPosition);
            }
            else
            {
                float dt = -c / b;
                Vector3 predictedTargetPosition = targetPosition + dt * targetVelocity;
                return PControllerForce(predictedTargetPosition);
            }
        }

        float discriminant = b * b - 4 * a * c;
        if(discriminant < 0.0f)
        {
            return PControllerForce(targetPosition);
        }
        else
        {
            float discriminantSqrt = Mathf.Sqrt(discriminant);
            float dt1 = (-b + discriminantSqrt) / (2 * a);
            float dt2 = (-b - discriminantSqrt) / (2 * a);
            float maxdt = Mathf.Max(dt1, dt2);
            float mindt = Mathf.Min(dt1, dt2);
            float dt;
            if (maxdt < 0)
            {
                dt = 0.0f;
            }
            else if (mindt > 0)
            {
                dt = mindt;
            }
            else
            {
                dt = maxdt;
            }
            Vector3 predictedTargetPosition = targetPosition + dt * targetVelocity;
            return PControllerForce(predictedTargetPosition);
        }
    }
}
