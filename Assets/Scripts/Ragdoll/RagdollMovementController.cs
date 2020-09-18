using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RagdollMovementController : MonoBehaviour
{
    Rigidbody rb;

    /* Transform of camera following player */
    public Transform cameraTrans;

    /* how fast player can move */
    float movementSpeed = 8.0f;

    /* movement vector of the player*/
    Vector3 movement = Vector3.zero;

    ConfigurableJoint confJoint;

    public Image cursorImage;

    public Transform handR;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        confJoint = GetComponent<ConfigurableJoint>();

        //Instantiate hook gun in player's hand
        GameObject hookGunGO = (GameObject)Instantiate(Resources.Load("Prefabs/HookGun"), 
            handR.position + 0.15f * handR.transform.forward, handR.rotation, handR);
        HookGun hookGun = hookGunGO.GetComponent<HookGun>();
        //hookGun.camWobbleDelegate = mainCamera.GetComponent<CameraController>().AddWobble;
        //hookGun.orientPlayerInAirDelegate = ApplyCentrifugalForce;
        hookGun.cursor.cursorImage = cursorImage;
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = PlayerInputManager.Instance.horizontal;
        float moveVertical = PlayerInputManager.Instance.vertical;
        movement = moveVertical * Vector3.ProjectOnPlane(cameraTrans.forward, Vector3.up).normalized +
                            moveHorizontal * cameraTrans.right;
    }

    private void FixedUpdate()
    {
        /* Player is on the ground */
        Vector3 vDesired = movementSpeed * movement.normalized;
        float k = (1 / Time.deltaTime) * 0.4f;
        Vector3 f = k * (vDesired - rb.velocity);
        rb.AddForce(f, ForceMode.Acceleration);

        /* Rotating character is done in RagdollAnimController */
        //Vector3 turningTorque = 100.0f * Vector3.Cross(transform.forward, cameraTrans.forward);
        //rb.AddTorque(turningTorque, ForceMode.Acceleration);

        
        confJoint.targetRotation = Quaternion.Inverse(Camera.main.transform.rotation);
    }

    public void MoveRagdoll(Vector3 force)
    {

    }
}
