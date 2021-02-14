using UnityEngine;
using CommonConfig;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    //UI更新時に子オブジェクトにアクセスしなくていいように
    //更新時に必要な要素だけこちらで変数に入れておく
    [System.Serializable]
    public struct VolumeButtonUI{
        public Image volumeImage;
        public Text volumeText;
    }

    [SerializeField] PlaySE playSE;
    public VolumeButtonUI volumeButtons;

    //音量調整後UIの更新をするイベント
    public delegate void VolumeChangeEventHandler();
    public event VolumeChangeEventHandler volumeUpdateEvent = () => {};

    //各ボリュームボタンクリック時の処理
    //各ボリューム値を受け取って設定 -> UIの表示更新
    public void ButtonClicked(int volume){
        playSE.PlaySound(AudioConfig.VolumeButton);
        PlayerPrefs.SetInt(PlayerPrefsKey.VolumeKey, volume);
        volumeUpdateEvent();
    }
}
