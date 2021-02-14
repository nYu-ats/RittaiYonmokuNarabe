using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class ReturnToGameButton : MonoBehaviour
{
    [SerializeField] GameObject giveUpConfirmPanel;
    [SerializeField] PlaySE playSE;

    public void OnClicked(){
        playSE.PlaySound(AudioConfig.ReturnIndex);
        giveUpConfirmPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
