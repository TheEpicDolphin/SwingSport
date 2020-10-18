using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRopeNode : MonoBehaviour
{
    public Vector3 previousPosition;

    private void Awake()
    {
        previousPosition = transform.position;
    }
}
