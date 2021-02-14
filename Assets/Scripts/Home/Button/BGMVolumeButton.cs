using UnityEngine;
using CommonConfig;

public class BGMVolumeButton: VolumeButton
{
    public override event VolumeChangeEventHandler volumeUpdateEvent = () => {};
    public override void ButtonClicked(int volume)
    {
        playSE.PlaySound(AudioConfig.VolumeButton);
        PlayerPrefs.SetInt(PlayerPrefsKey.BgmVolumeKey, volume);
        volumeUpdateEvent();
    }
}