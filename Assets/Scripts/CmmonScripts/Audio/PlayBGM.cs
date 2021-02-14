using UnityEngine;
using CommonConfig;

public class PlayBGM : MonoBehaviour
{
    [SerializeField] VolumeButton[] volumeButton;
    private float volume = 0.0f;
    private float adjustVolumeNum = 2.0f;
    void Start(){
        UpdateVolume();
        foreach(VolumeButton vBtn in volumeButton){
            vBtn.volumeUpdateEvent += UpdateVolume;
        }
        this.GetComponent<AudioSource>().Play();
    }

    private void UpdateVolume(){
        volume = PlayerPrefs.GetInt(PlayerPrefsKey.VolumeKey) /AudioConfig.MaxVolume /adjustVolumeNum;
        this.GetComponent<AudioSource>().volume = volume;
    }
}