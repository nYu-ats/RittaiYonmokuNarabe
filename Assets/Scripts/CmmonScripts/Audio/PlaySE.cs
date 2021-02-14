using UnityEngine;
using CommonConfig;

interface IPlaySE{
    void PlaySound(int clipNumber);
}

public class PlaySE : MonoBehaviour, IPlaySE
{
    [SerializeField] VolumeButton[] volumeButton;
    private float seVolume = 0.0f;
    void Start(){
        UpdateVolume();
        foreach(VolumeButton vBtn in volumeButton){
            vBtn.volumeUpdateEvent += UpdateVolume;
        }
    }

    [SerializeField] AudioClip[] audioClips;

    public void PlaySound(int clipNumber){
        this.GetComponent<AudioSource>().PlayOneShot(audioClips[clipNumber]);
    }

    private void UpdateVolume(){
        seVolume = PlayerPrefs.GetInt(PlayerPrefsKey.VolumeKey) / AudioConfig.MaxVolume;
        this.GetComponent<AudioSource>().volume = seVolume;
    }
}
