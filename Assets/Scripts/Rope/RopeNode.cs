using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static void ApplyConstraint(VerletParticle vp1, VerletParticle vp2)
    {

    }

    static void ApplyConstraint(RopeAttachment ra1, RopeAttachment ra2)
    {

    }

    static void ApplyConstraint(RopeAttachment ra, VerletParticle verletParticle)
    {

    }

    static void ApplyConstraint(VerletParticle vp, RopeAttachment ra)
    {
        ApplyConstraint(ra, vp);
    }

    
}
