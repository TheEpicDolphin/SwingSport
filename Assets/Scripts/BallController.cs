using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public int numPlayers = 1;
    public float influnceRange = 40.0f;

    Rigidbody ballRb;

    private void Awake()
    {
        ballRb = GetComponent<Rigidbody>();
        ballRb.isKinematic = false;
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
        Physics.OverlapSphereNonAlloc(transform.position, influnceRange, colliders, magnetogloveLayerMask);
        foreach (Collider collider in colliders)
        {
            MagnetoGlove magnetoGlove = collider.GetComponent<MagnetoGlove>();
            Rigidbody magnetoGloveRb = collider.GetComponent<Rigidbody>();
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
        if(transform.parent != null)
        {
            Vector3 ballVel = ballRb.velocity;

            ballRb.isKinematic = true;
            ballRb.MovePosition(other.transform.position);
            /* give triggering entity possession of ball */
            transform.parent = other.transform;

            MagnetoGlove magnetoGlove = other.GetComponent<MagnetoGlove>();
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
        }
    }
}
