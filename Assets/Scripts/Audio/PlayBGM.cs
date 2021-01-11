using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : MonoBehaviour
{
    private const float maxVolume = 2.0f;
    private float volume = 0.0f;
    void Start(){
        volume = PlayerPrefs.GetInt("Volume") / maxVolume;
        this.GetComponent<AudioSource>().volume = volume;
        this.GetComponent<AudioSource>().Play();
    }
}