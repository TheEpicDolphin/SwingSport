using UnityEngine;

public class BallGrabbing : MonoBehaviour
{

    public Transform hasBallVisual;
    bool hasBall = false;

    public float ballLaunchForce = 100.0f;
    public const float maxGrabTime = 3.0f;

    public float ballUngrabbableTimeLimit = 3.0f;

    private float currentGrabTime = 0.0f;

    void Start()
    {
        hasBall = false;
        hasBallVisual.gameObject.SetActive(false);
        hasBallVisual.gameObject.GetComponent<Rigidbody>().constraints =
            RigidbodyConstraints.FreezePositionX | 
            RigidbodyConstraints.FreezePositionZ | 
            RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
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
        
    }

    private void letGoOfBall(bool shouldLaunch)
    {
        Ray camRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 newBounceBallStartPos = transform.position + ((camRay.direction / camRay.direction.magnitude) * 7.5f);

        GameObject bounceBall = (GameObject)Instantiate(
            Resources.Load("Prefabs/BounceBall"),
            gameObject.transform.position,
            gameObject.transform.rotation
        );

        bounceBall.transform.position = newBounceBallStartPos;
        bounceBall.layer = LayerMask.NameToLayer("HookableLayer");

        if (shouldLaunch)
        {
            Debug.Log("User launched ball.");

            Vector3 ballLaunchForceVector = (camRay.direction / camRay.direction.magnitude) * ballLaunchForce;

            Debug.Log("Launching ball with force vector: " + ballLaunchForceVector);

            bounceBall.GetComponent<Rigidbody>().AddForce(ballLaunchForceVector, ForceMode.Impulse);
        }

        bounceBall.GetComponent<GrabbableObject>().setUngrabbable(ballUngrabbableTimeLimit);

        turnOffBallVisual();
    }

    private void turnOffBallVisual()
    {
        hasBallVisual.gameObject.SetActive(false);
        hasBall = false;
        currentGrabTime = 0.0f;
        hasBallVisual.gameObject.GetComponent<Renderer>().material.color = Color.green;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BounceBall")
        {
            Debug.Log("Detected a bounce ball entered grabbing collider: " + other.gameObject.name);

            if (other.gameObject.GetComponent<GrabbableObject>().isCurrentlyGrabbable() && !hasBall)
            {

                Debug.Log("Didn't already have ball, destroying game object and placing on back.");

                hasBall = true;
                Destroy(other.gameObject);

                hasBallVisual.gameObject.SetActive(true);

                currentGrabTime = maxGrabTime;
            }

        }
    }
}
