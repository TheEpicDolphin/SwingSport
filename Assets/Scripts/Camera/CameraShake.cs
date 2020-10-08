using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;
	private float currentShakeAmount;

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
			camTransform.localPosition = originalPos + Random.insideUnitSphere * currentShakeAmount;
		}
		else
		{
			camTransform.localPosition = originalPos;
		}
	}

	public void stopShaking()
    {
		shouldShake = false;
    }

	public void startShaking(float shakeAmount)
    {
		currentShakeAmount = shakeAmount;
		shouldShake = true;
    }
}