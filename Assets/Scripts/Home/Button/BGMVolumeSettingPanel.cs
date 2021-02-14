using UnityEngine.UI;
using CommonConfig;

//VolumeSettingPanelを継承
//Start時のprefsKey設定行にて、BGM用のキーを設定するようにする
public class BGMVolumeSettingPanel : VolumeSettingPanel
{
    protected override void Start()
    {
        prefsKey = PlayerPrefsKey.BgmVolumeKey;
        //イベントハンドラーを追加
        foreach(Button btn in volumeButtons){
            btn.GetComponent<VolumeButton>().volumeUpdateEvent += ChangeVolumeFocus;
        }
        ChangeVolumeFocus(); //パネルを開いた時点での音量にフォーカスする
    }
}
