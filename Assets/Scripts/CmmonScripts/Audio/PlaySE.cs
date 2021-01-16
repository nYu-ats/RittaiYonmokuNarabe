using UnityEngine;
using CommonConfig;

interface IPlaySE{
    void PlaySound(int clipNumber);
}

public class PlaySE : MonoBehaviour, IPlaySE
{
    private float volume = 0.0f;
    void Start(){
        volume = PlayerPrefs.GetInt(PlayerPrefsKey.VolumeKey) / AudioConfig.MaxVolume;
        this.GetComponent<AudioSource>().volume = volume;
    }

    [SerializeField] AudioClip[] audioClips;

    public void PlaySound(int clipNumber){
        this.GetComponent<AudioSource>().PlayOneShot(audioClips[clipNumber]);
    }
}
