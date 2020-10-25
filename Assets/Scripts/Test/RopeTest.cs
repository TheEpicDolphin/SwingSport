using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTest : MonoBehaviour
{
    Rope rope;
    // Start is called before the first frame update
    void Start()
    {
        rope = Rope.CreateTautRope(new Vector3(-2.1f, 1, 0), new Vector3(2.1f, 1, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
