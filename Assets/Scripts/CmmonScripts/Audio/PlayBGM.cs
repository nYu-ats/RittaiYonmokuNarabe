using UnityEngine;
using CommonConfig;

public class PlayBGM : MonoBehaviour
{
    private float volume = 0.0f;
    void Start(){
        volume = PlayerPrefs.GetInt(PlayerPrefsKey.VolumeKey) /AudioConfig.MaxVolume;
        this.GetComponent<AudioSource>().volume = volume;
        this.GetComponent<AudioSource>().Play();
    }
}