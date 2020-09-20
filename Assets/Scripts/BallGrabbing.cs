using UnityEngine;

public class BallGrabbing : MonoBehaviour
{

    public Transform hasBallVisual;
    bool hasBall = false;

    public float ballLaunchForce = 100.0f;

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (hasBall)
            {
                Debug.Log("User launched ball.");

                Ray camRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                /* Prevent raycast from hitting something in front of camera but behind gun */
                Vector3 newBounceBallStartPos = transform.position + ((camRay.direction / camRay.direction.magnitude) * 7.5f);

                GameObject bounceBall = (GameObject) Instantiate(
                    Resources.Load("Prefabs/BounceBall"),
                    gameObject.transform.position, 
                    gameObject.transform.rotation
                );

                bounceBall.transform.position = newBounceBallStartPos;

                Vector3 ballLaunchForceVector = (camRay.direction / camRay.direction.magnitude) * ballLaunchForce;

                Debug.Log("Launching ball with force vector: " + ballLaunchForceVector);

                bounceBall.GetComponent<Rigidbody>().AddForce(ballLaunchForceVector, ForceMode.Impulse);

                hasBallVisual.gameObject.SetActive(false);
                hasBall = false;
            }
}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BounceBall")
        {
            Debug.Log("Detected a bounce ball entered grabbing collider: " + other.gameObject.name);

            if (!hasBall)
            {

                Debug.Log("Didn't already have ball, destroying game object and placing on back.");

                hasBall = true;
                Destroy(other.gameObject);

                hasBallVisual.gameObject.SetActive(true);
            }

        }
    }
}
