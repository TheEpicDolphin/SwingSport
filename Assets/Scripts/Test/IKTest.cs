using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTest : MonoBehaviour
{
    public Transform target1;
    public Transform target2;
    public Transform root;
    public Transform handR;
    public Transform handL;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        FABRIKSolver fabrikSolver = new FABRIKSolver(root,
            new Dictionary<Transform, Vector3>
            {
                { handR, target1.position },
                { handL, target2.position }
            });
        fabrikSolver.Solve();
    }
}
