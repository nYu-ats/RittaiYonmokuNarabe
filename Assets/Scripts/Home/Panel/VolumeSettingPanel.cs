using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class VolumeSettingPanel : MonoBehaviour
{
    delegate void VolumeChangeEventHandler(); //イベントを発生させるためのdelegate
    private event VolumeChangeEventHandler uiUpdateEvent = () => {};
    //インスペクターで扱いやすくするため、ボタン画像とテキストをまとめる
    [System.Serializable]
    public struct VolumeButtonUI{
        public Image volumeImage;
        public Text volumeText;
    }
    [SerializeField] VolumeButtonUI[] volumeButtons;

    void Start()
    {
        uiUpdateEvent += ChangeVolumeFocus; //イベントハンドラー追加
        uiUpdateEvent(); 
    }

    //各ボリュームボタンがクリックされたときの処理
    //各ボリューム値を受け取って設定 -> UIの表示更新イベントを発生させる
    public void ButtonClicked(int volume){
        PlayerPrefs.SetInt(PlayerPrefsKey.VolumeKey, volume);
        uiUpdateEvent();
    }

    //設定されている音量をフォーカスさせる
    private void ChangeVolumeFocus(){
        int focueIndex = PlayerPrefs.GetInt(PlayerPrefsKey.VolumeKey);
        for(int index=0; index < volumeButtons.Length; index++){
            if(index != focueIndex){
                volumeButtons[index].volumeImage.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                volumeButtons[index].volumeText.GetComponent<Text>().color = new Color(0, 0, 0, 1);
            }
            else{
                volumeButtons[index].volumeImage.GetComponent<Image>().color = new Color(0, 0, 0, 1);
                volumeButtons[index].volumeText.GetComponent<Text>().color = new Color(255, 255, 255, 1);
            }
        }
    }
}
