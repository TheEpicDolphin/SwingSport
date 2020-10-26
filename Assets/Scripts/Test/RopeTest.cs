using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTest : MonoBehaviour
{
    Rope rope;
    public RopeAttachment ra;

    public RopeAttachment hangerRA;
    public RopeAttachment sphereRA;
    // Start is called before the first frame update
    void Start()
    {
        //rope = Rope.CreateTautRope(new Vector3(-2.1f, 1, 0), new Vector3(2.1f, 1, 0));
        //rope.Attach(ra);
        rope = Rope.CreateTautRope(sphereRA.AttachmentPoint(), hangerRA.AttachmentPoint());
        rope.Attach(hangerRA);
        rope.Attach(sphereRA);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rope.RemoveRope(sphereRA.restPosition + 1e-4f, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rope.InsertRope(sphereRA.restPosition + 1e-4f, Time.deltaTime);
        }
    }
}
