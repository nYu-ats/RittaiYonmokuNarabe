using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IPlaySE{
    void PlaySound(int clipNumber);
}

public class PlaySE : MonoBehaviour, IPlaySE
{
    private const float maxVolume = 2.0f;
    private float volume = 0.0f;
    void Start(){
        volume = PlayerPrefs.GetInt("Volume") / maxVolume;
        this.GetComponent<AudioSource>().volume = volume;
    }

    [SerializeField] AudioClip[] audioClips;

    public void PlaySound(int clipNumber){
        this.GetComponent<AudioSource>().PlayOneShot(audioClips[clipNumber]);
    }
}
