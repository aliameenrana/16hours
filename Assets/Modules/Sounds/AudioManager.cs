using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip levelComplete;
    public AudioClip match;
    public AudioClip mismatch;
    public AudioClip flip;

    private AudioSource aSource;

    private void Awake()
    {
        instance = this;
        aSource = GetComponent<AudioSource>();
    }

    public void LevelComplete()
    {
        aSource.clip = levelComplete;
        aSource.Play();
    }

    public void Match()
    {
        aSource.clip = match;
        aSource.Play();
    }

    public void Mismatch()
    {
        aSource.clip = mismatch;
        aSource.Play();
    }

    public void Flip()
    {
        aSource.clip = flip;
        aSource.Play();
    }
}