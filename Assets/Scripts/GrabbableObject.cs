using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{

    private bool grabbable;

    private float currentUngrabbablePeriod = 1.0f;
    private float currentUngrabbableTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!grabbable)
        {
            currentUngrabbableTime -= Time.deltaTime;

            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.red, currentUngrabbableTime / currentUngrabbablePeriod);

            if (currentUngrabbableTime < 0)
            {
                grabbable = true;
                currentUngrabbableTime = 0.0f;
            }

        }
    }

    public bool isCurrentlyGrabbable()
    {
        return grabbable;
    }

    public void setUngrabbable(float timePeriod)
    {

        grabbable = false;
        currentUngrabbablePeriod = timePeriod;
        currentUngrabbableTime = currentUngrabbablePeriod;
    }

}
