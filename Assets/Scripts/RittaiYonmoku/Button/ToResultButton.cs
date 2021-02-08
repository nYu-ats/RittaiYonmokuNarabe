using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;
using Cysharp.Threading.Tasks;

public class ToResultButton : MonoBehaviour
{
    [SerializeField] GameObject gameResultPanel;
    [SerializeField] GameObject giveUpConfirmPanel;
    [SerializeField] GameController gameController;
    [SerializeField] SyncBoardStatus syncBoardStatus;

    public void OnClicked(){
        //ギブアップは自身のターンにしかできないため、ゲームリザルトに相手を勝者としてセットする
        gameController.CurrentTurn += gameController.Rival;
        gameResultPanel.SetActive(true);
        giveUpConfirmPanel.SetActive(false);
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            //マルチプレイの場合は相手にギブアップ通知をする
            syncBoardStatus.GiveUpAction();
        }
    }
}
