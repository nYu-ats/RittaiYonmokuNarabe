using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class GiveUpButton : MonoBehaviour
{
    [SerializeField] GameObject giveUpConfirmPanel;
    [SerializeField] GameController gameController;

    public void OnClicked(){
        giveUpConfirmPanel.SetActive(true);
        if(gameController.PlayMode == GameRule.SoloPlayMode){
            Time.timeScale = 0;
        }
        //マルチプレイの場合は相手との時間制限の同期がとれなくなるので
        //制限時間の停止は行わない
    }
}
