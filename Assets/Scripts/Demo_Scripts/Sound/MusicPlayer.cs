using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public AudioClip musicClip;
    public AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio.PlayOneShot(musicClip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
