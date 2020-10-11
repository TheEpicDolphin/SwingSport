using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAttachment : MonoBehaviour
{

    public GameObject viewGO;
    public GameObject playerGO;
    public GameObject vehicleGO;
    public GameObject playerHipsGO;
    public GameObject playerBody;
    private Component[] playerBodyRenderers;

    public PlayerCamera cameraScript;

    private ActiveRagdoll playerRagdollScript;

    public float attachmentTime = 30.0f;

    // Start is called before the first frame update
    void Start()
    {

        playerBodyRenderers = playerBody.GetComponentsInChildren(typeof(Renderer));

        IEnumerator attach = AttachCharacterToVehicle(0);
        StartCoroutine(attach);
        IEnumerator detach = DetachCharacterFromVehicle(attachmentTime);
        StartCoroutine(detach);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AttachCharacterToVehicle(float delay)
    {
        yield return new WaitForSeconds(delay);

        viewGO.transform.parent = vehicleGO.transform;
        playerGO.transform.parent = vehicleGO.transform;
        cameraScript.setThirdPerson(false);
        playerBody.SetActive(false);
    }

    IEnumerator DetachCharacterFromVehicle(float delay)
    {
        yield return new WaitForSeconds(delay);

        viewGO.transform.parent = null;
        playerGO.transform.parent = null;
        cameraScript.setThirdPerson(true);
        playerBody.SetActive(true);
    }
}
