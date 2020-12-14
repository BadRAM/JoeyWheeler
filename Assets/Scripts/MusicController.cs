using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip PreLoop;
    public AudioClip TensionBuild;
    public AudioClip BossFight;

    private AudioSource _audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (GameInfo.TimeToBossSpawn > TensionBuild.length)
        {
            _audioSource.clip = PreLoop;
            _audioSource.time = PreLoop.length - (GameInfo.TimeToBossSpawn - TensionBuild.length) % PreLoop.length;
            _audioSource.Play();
        }
        else
        {
            _audioSource.clip = TensionBuild;
            _audioSource.time = TensionBuild.length - GameInfo.TimeToBossSpawn;
            _audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameInfo.TimeToBossSpawn < TensionBuild.length && _audioSource.clip == PreLoop)
        {
            _audioSource.time = 0;
            _audioSource.clip = TensionBuild;
            _audioSource.Play();
        }

        if (GameInfo.TimeToBossSpawn <= 0 && _audioSource.clip == TensionBuild)
        {
            _audioSource.time = 0;
            _audioSource.clip = BossFight;
            _audioSource.Play();
        }
    }
}
