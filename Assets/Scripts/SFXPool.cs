using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(AudioSource))]
public class SFXPool : MonoBehaviour
{
    public List<AudioClip> clips;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        _audioSource.Stop();
        _audioSource.clip = clips[Random.Range(0, clips.Count - 1)];
        _audioSource.Play();
    }
}
