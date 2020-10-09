using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BlockMover : MonoBehaviour
{
    bool shouldMove = false;
    float timeSinceLastMove;
    float timeMoving;
    public float moveTime = 5.0f;
    private Vector3 lastStartPosition;
    public float moveInterval = 5.0f;
    public float moveOffset = 20.0f;
    public float cameraShakeAmount = 0.3f;
    public bool shakingEnabled = true;

    public GameObject playerViewer;
    private CameraShake shaker;
    private bool firstEnterShaking = true;
    private bool firstEnterNonShaking = true;
    private bool shouldShake = false;

    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastMove = moveInterval;
        shaker = playerViewer.GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            //Debug.Log("Block should be moving for " + (moveTime - timeMoving) + " more seconds, current position is " + transform.position + ", ratio is " + (moveTime - timeMoving) / moveTime);
            if (timeMoving <= 0.0f)
            {
                shouldMove = false;
                timeSinceLastMove = moveInterval;
            } else
            {
                timeMoving -= Time.deltaTime;
                transform.position = Vector3.Lerp(lastStartPosition, lastStartPosition + new Vector3(0, 0, -moveOffset), (moveTime - timeMoving) / moveTime);

                Debug.Log((moveTime - timeMoving) / moveTime + ", " + shouldShake);

                if (shakingEnabled)
                {
                    if (//(((moveTime - timeMoving) / moveTime) > 0.4f && ((moveTime - timeMoving) / moveTime) < 0.6f) ||
                        (((moveTime - timeMoving) / moveTime) < 0.3f || ((moveTime - timeMoving) / moveTime) > 0.7f)
                        )
                    {
                        if (firstEnterShaking)
                        {
                            firstEnterShaking = false;
                            if (shouldShake)
                            {
                                shaker.startShaking(cameraShakeAmount);
                                shouldShake = false;
                            }
                            else
                            {
                                shouldShake = true;
                            }
                        }
                        else
                        {

                        }

                        firstEnterNonShaking = true;

                    }
                    else
                    {
                        if (firstEnterNonShaking)
                        {
                            firstEnterNonShaking = false;
                            shaker.stopShaking();
                        }
                        else
                        {

                        }

                        firstEnterShaking = true;
                    }
                }
            }
        }
        else
        {
            //Debug.Log("Block should not be moving for " + (moveInterval - timeSinceLastMove) + " more seconds, current position is " + transform.position);
            if (timeSinceLastMove <= 0.0f)
            {
                shouldMove = true;
                timeMoving = moveTime;
                lastStartPosition = transform.position;
            } else
            {
                timeSinceLastMove -= Time.deltaTime;
            }
        }
    }

}
