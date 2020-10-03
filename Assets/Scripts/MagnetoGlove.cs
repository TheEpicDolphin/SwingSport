using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make it adhere to IItem interface
public class MagnetoGlove : MonoBehaviour
{
    float shrunkBallRadius = 1.0f;

    public Transform ballTarget;

    Renderer ballGrabbingRegionRenderer;

    /* This can vary as glove loses magnetic power */
    public float Strength { get; private set; } = 20.0f;

    public bool IsMagnetizing { get; private set; } = false;

    public bool visualizeGrabbingRegion = false;

    /* Holds the ball in place when it is close enough to hand */
    FixedJoint ballHolder;

    Rigidbody gloveRb;

    Rigidbody handRb;

    float constantRange = 1.0f;
    
    float breakForce = 1000.0f;

    bool equipped = true;

    SphereCollider sphereCollider;

    float ballThrowStrength = 100.0f;

    float maxThrowDistance = 100.0f;

    Ball possessedBall;

    Vector3 lastBallTargetPosition = Vector3.zero;
    public Vector3 ballTargetVelocity = Vector3.zero;

    private void Awake()
    {
        GameObject ballTargetGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(ballTargetGO.GetComponent<Collider>());
        ballGrabbingRegionRenderer = ballTargetGO.GetComponent<Renderer>();

        ballTarget = ballTargetGO.transform;
        ballTarget.parent = transform;
        ballTarget.position = transform.position + shrunkBallRadius * transform.forward;
        ballTarget.rotation = Quaternion.identity;

        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = 0.5f;
        sphereCollider.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer("MagnetoGlove");
    }

    public void Equip(Transform parent, Vector3 position, Quaternion rotation, bool usePhysics)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.parent = parent;
        handRb = parent.GetComponent<Rigidbody>();
    }

    public void ApplyForceOnHand(Vector3 force, ForceMode forceMode)
    {
        handRb.AddForceAtPosition(force, transform.position, forceMode);
    }

    // Update is called once per frame
    void Update()
    {
        ballGrabbingRegionRenderer.enabled = visualizeGrabbingRegion;
    }

    public void Handle(Player player)
    {
        IsMagnetizing = Input.GetMouseButton(0);

        /* This belongs in Update because FixedUpdate will sometimes miss the Q down press */
        if (player.input.QDown && possessedBall)
        {
            ThrowBall();
        }
    } 

    private void FixedUpdate()
    {
        ballTargetVelocity = (ballTarget.position - lastBallTargetPosition) / Time.fixedDeltaTime;
        lastBallTargetPosition = ballTarget.position;
    }

    private void OnDestroy()
    {
        Destroy(ballTarget.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!possessedBall)
        {
            Ball ball = other.GetComponent<Ball>();
            if (ball && IsMagnetizing)
            {
                possessedBall = ball;
                Rigidbody ballRb = possessedBall.GetComponent<Rigidbody>();
                Vector3 ballVel = ballRb.velocity;
                Grab(ball);
                ApplyForceOnHand(ballVel, ForceMode.Impulse);
            }
        }
    }

    public void ThrowBall()
    {
        Ball ball = possessedBall;
        Release();

        Ray camRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        float startT = Vector3.Dot(transform.position - camRay.origin, camRay.direction);
        RaycastHit hit;
        Vector3 targetPos = Vector3.zero;
        if (Physics.Raycast(new Ray(camRay.GetPoint(startT), camRay.direction), out hit, maxThrowDistance))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = camRay.GetPoint(maxThrowDistance);
        }
        Vector3 throwDirection = (targetPos - ball.transform.position).normalized;
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        ballRb.AddForce(ballThrowStrength * throwDirection, ForceMode.Impulse);
    }

    public void Grab(Ball ball)
    {
        possessedBall = ball;
        ball.Possessor = this;
        ball.transform.position = ballTarget.position;
    }

    public void Release()
    {
        possessedBall.Possessor = null;
        possessedBall = null;
    }

}
