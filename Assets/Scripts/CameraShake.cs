using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;

	Vector3 originalPos;

	bool shouldShake;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	void Update()
	{
		if (shouldShake)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
		}
		else
		{
			camTransform.localPosition = originalPos;
		}
	}

	public void setShouldShake(bool val)
    {
		shouldShake = val;
    }
}