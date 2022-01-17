using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public AudioClip blockSFX;
    public AudioClip stunnedSFX;
    public AudioClip attackSFX_001;
    private AudioSource audioSource;
    public AudioSource attackAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(string sfx)
    {
        switch (sfx)
        {
            case "block":
                audioSource.clip = blockSFX;
                audioSource.pitch = 1;
                audioSource.Play();
                break;
            case "stunned":
                audioSource.clip = stunnedSFX;
                audioSource.pitch = 0.23f;
                audioSource.Play();
                break;
            case "attack001":
                audioSource.clip = attackSFX_001;
                if(!audioSource.isPlaying)
                    audioSource.pitch = 1;
                    audioSource.Play();
                break;
            default:
                break;
        }
    }

    public bool isClipPlaying(AudioClip clip)
    {
        if(audioSource.clip == clip)
        {
            if(audioSource.isPlaying)
            {
             return true;   
            }
        } 
        else
        {
            return false;
        }

        return false;
    }
}
