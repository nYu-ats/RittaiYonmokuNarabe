using UnityEngine;
using CommonConfig;

public class PlayBGM : MonoBehaviour
{
    [SerializeField] VolumeButton[] volumeButton;
    [SerializeField] float fadeSeconds = 1.0f;
    private bool isFadeIn = true;
    private bool isFadeOut = false;
    public bool IsFadeOut{
        set {isFadeOut = value;}
        get {return isFadeOut;}
        }
    private float fadeTime = 0.0f;
    private float volume = 0.0f;

    void Start(){
        UpdateVolume();
        foreach(VolumeButton vBtn in volumeButton){
            vBtn.volumeUpdateEvent += UpdateVolume;
        }
        this.GetComponent<AudioSource>().volume = 0; //フェードインのため音量0で初期化する
        this.volume = PlayerPrefs.GetInt(PlayerPrefsKey.BgmVolumeKey) / AudioConfig.MaxBgmVolume;
        this.GetComponent<AudioSource>().Play();
    }

    void Update(){
        if(isFadeIn){
            FadeIn();
        }else if(isFadeOut){
            FadeOut();
        }
    }

    private void UpdateVolume(){
        volume = PlayerPrefs.GetInt(PlayerPrefsKey.BgmVolumeKey) / AudioConfig.MaxBgmVolume;
        this.GetComponent<AudioSource>().volume = volume;
    }

    private void FadeIn(){
        fadeTime += Time.deltaTime;
        if(fadeTime >= fadeSeconds){
            fadeTime = 0.0f;
            isFadeIn = false;
        }
        else{
            this.GetComponent<AudioSource>().volume = fadeTime / fadeSeconds * volume;
        }
    }

    public void FadeOut(){
        fadeTime += Time.deltaTime;
        if(fadeTime >= fadeSeconds){
            fadeTime = 0.0f;
            isFadeOut = false;
        }
        else{
            this.GetComponent<AudioSource>().volume = volume - fadeTime / fadeSeconds * volume;
        }
    }
}