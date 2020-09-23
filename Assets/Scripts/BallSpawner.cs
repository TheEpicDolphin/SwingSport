using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{

    public int maxBalls = 20;
    public float launchFrequency = 5.0f;

    public float minXVelocity = 0.0f;
    public float minYVelocity = 0.0f;
    public float minZVelocity = 0.0f;

    public float maxXVelocity = 100.0f;
    public float maxYVelocity = 100.0f;
    public float maxZVelocity = 100.0f;

    Queue<GameObject> balls;

    // Start is called before the first frame update
    void Start()
    {
        balls = new Queue<GameObject>();

        InvokeRepeating("LaunchBall", Random.Range(0.0f, 5.0f), launchFrequency);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LaunchBall()
    {
        Vector3 ballVelocity = new Vector3(
            Random.Range(minXVelocity, maxXVelocity),
            Random.Range(minYVelocity, maxYVelocity),
            Random.Range(minZVelocity, maxZVelocity)
            );

        //Debug.Log("Launching ball with velocity " + ballVelocity);

        GameObject bounceBall = (GameObject)Instantiate(Resources.Load("Prefabs/BounceBall"),
            gameObject.transform.position, gameObject.transform.rotation);
        bounceBall.GetComponent<Rigidbody>().velocity = ballVelocity;

        balls.Enqueue(bounceBall);

        if (balls.Count >= maxBalls)
        {
            GameObject firstBall = balls.Dequeue();
            Destroy(firstBall);
        }
    }

}
