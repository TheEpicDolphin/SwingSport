using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public int numPlayers = 1;
    public float influnceRange = 40.0f;

    Rigidbody ballRb;
    SphereCollider ballCollider;

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
                Vector3 homingForce = InverseSquareForceLaw(magnetoGlove.Strength, r);
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

    private void OnTriggerStay(Collider other)
    {
        if(transform.parent == null)
        {
            MagnetoGlove magnetoGlove = other.GetComponent<MagnetoGlove>();
            ballCollider.enabled = false;

            Vector3 ballVel = ballRb.velocity;

            ballRb.isKinematic = true;
            ballRb.MovePosition(magnetoGlove.ballTarget.position);
            /* give triggering entity possession of ball */
            transform.parent = other.transform;
            /* Apply force to glove after catching */
            magnetoGlove.ApplyForceOnHand(ballVel, ForceMode.Impulse);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /* Check if ball is leaving the entity that previously had possession of the ball */
        if (other.transform == transform.parent)
        {
            ballRb.isKinematic = false;
            transform.parent = null;

            ballCollider.enabled = true;
        }
    }
}
