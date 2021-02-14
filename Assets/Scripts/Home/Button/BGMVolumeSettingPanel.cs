using UnityEngine.UI;
using CommonConfig;

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
