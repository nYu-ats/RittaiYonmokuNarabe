using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class VolumeSettingPanel : MonoBehaviour
{
    public Button[] volumeButtons;
    //フォーカス/非フォーカスの色をインスペクターから設定できるようにする
    [SerializeField] Color focusedTextColor = new Color(255, 255, 255, 1);
    [SerializeField] Color focusedImageColor = new Color(100, 255, 0, 1);
    [SerializeField] Color notFocusedTextColor = new Color(50, 50, 50, 1);
    [SerializeField] Color notFocusedImageColor = new Color(89, 89, 89, 1);
    protected string prefsKey;

    protected virtual void Start()
    {
        prefsKey = PlayerPrefsKey.VolumeKey;
        //イベントハンドラーを追加
        foreach(Button btn in volumeButtons){
            btn.GetComponent<VolumeButton>().volumeUpdateEvent += ChangeVolumeFocus;
        }
        ChangeVolumeFocus(); //パネルを開いた時点での音量にフォーカスする
    }

    //設定されている音量にフォーカスする
    protected void ChangeVolumeFocus(){
        int focueIndex = PlayerPrefs.GetInt(prefsKey);
        for(int index=0; index < volumeButtons.Length; index++){
            if(index != focueIndex){
                volumeButtons[index].GetComponent<VolumeButton>().volumeButtons.volumeImage.GetComponent<Image>().color = notFocusedImageColor;
                volumeButtons[index].GetComponent<VolumeButton>().volumeButtons.volumeText.GetComponent<Text>().color = notFocusedTextColor;
            }
            else{
                volumeButtons[index].GetComponent<VolumeButton>().volumeButtons.volumeImage.GetComponent<Image>().color = focusedImageColor;
                volumeButtons[index].GetComponent<VolumeButton>().volumeButtons.volumeText.GetComponent<Text>().color = focusedTextColor;
            }
        }
    }
}
