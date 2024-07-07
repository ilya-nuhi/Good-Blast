using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Sounds")]
    [SerializeField] AudioClip popSoundFX;

    [Range(0,1)]
    public float fxVolume = 0.5f;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayPoPSound(){
        
        audioSource.PlayOneShot(popSoundFX, fxVolume);
        
    }
}
