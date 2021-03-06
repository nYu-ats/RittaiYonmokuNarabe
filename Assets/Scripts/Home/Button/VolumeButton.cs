﻿using UnityEngine;
using CommonConfig;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    //UI更新のため自身の画像とテキストを保持しておく
    [System.Serializable]
    public struct VolumeButtonUI{
        public Image volumeImage;
        public Text volumeText;
    }

    public PlaySE playSE;
    public VolumeButtonUI volumeButtons;

    //音量調整後UIの更新をするイベント
    public delegate void VolumeChangeEventHandler();
    public virtual event VolumeChangeEventHandler volumeUpdateEvent = () => {};

    //各ボリュームボタンクリック時の処理
    //各ボリューム値を受け取って設定 -> UIの表示更新
    public virtual void ButtonClicked(int volume){
        playSE.PlaySound(AudioConfig.VolumeButton);
        PlayerPrefs.SetInt(PlayerPrefsKey.VolumeKey, volume);
        volumeUpdateEvent();
    }
}
