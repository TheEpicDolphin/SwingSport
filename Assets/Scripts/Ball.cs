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

    /*
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
                transform.parent = value.transform;
                possessor = value;
            }
            else
            {
                transform.parent = null;
                possessor = null;
            }
            
        }
    }
    */

    float maxSpeed = 20.0f;
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
                Vector3 homingForce = PControllerForce(magnetoGlove.ballTarget.position);
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

    public void Grab(Transform parent, Vector3 position)
    {
        ballCollider.enabled = false;
        ballRb.isKinematic = true;
        transform.position = position;
        transform.parent = parent;
    }

    public void Release()
    {
        transform.parent = null;
        ballRb.isKinematic = false;
        ballCollider.enabled = true;
    }

}
