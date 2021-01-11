using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

delegate void VolumeChangeEventHandler();

public class VolumeSettingPanel : MonoBehaviour
{
    

    private event VolumeChangeEventHandler changeEvent = () => {};

    [SerializeField] VolumeButton[] volumeButtons;

    void Start()
    {
        changeEvent += ChangeVolumeNumberFocus;
        ChangeVolumeNumberFocus(); //音量フォーカス表示の初期化
    }

    public void ButtonClicked(int volume){
        PlayerPrefs.SetInt("Volume", volume);
        changeEvent();
    }

    //ボタン画像とテキストをまとめて管理できるようにする
    [System.Serializable]
    public struct VolumeButton{
        public Image volumeImage;
        public Text volumeText;
    }
    private void ChangeVolumeNumberFocus(){
        int focueIndex = PlayerPrefs.GetInt("Volume");
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
