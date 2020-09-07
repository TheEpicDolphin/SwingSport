using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CameraWobbleDelegate(float intensity);

public class CameraController : MonoBehaviour
{
    
    public Transform target;

    /* how rigid the camera movement feels */
    float cameraRigidness = 30.0f;
    public float shakeMultiplier = 0.2f;
    float wobbleIntensity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddWobble(float intensity)
    {
        wobbleIntensity = intensity;
    }

    private void FixedUpdate()
    {
        //Camera shake
        Vector3 shookCameraPosition = target.position + shakeMultiplier * wobbleIntensity * Random.insideUnitSphere;
        transform.position = Vector3.Slerp(transform.position, shookCameraPosition, Mathf.Min(cameraRigidness * Time.fixedDeltaTime, 1.0f));
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Mathf.Min(cameraRigidness * Time.fixedDeltaTime, 1.0f));

        //wobble intensity is decreased smoothly
        wobbleIntensity = Mathf.Lerp(wobbleIntensity, 0.0f, 2.0f * Time.fixedDeltaTime);
    }
}
