using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class GameStatusPanel : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] GameObject checkMateUI;
    [SerializeField] GameObject gameResultPanel;
    [SerializeField] int statusDisplayTime = 3000;
    [SerializeField] SyncBoardStatus syncBoardStatus;
    [SerializeField] GameObject giveUpUI;
    void Start()
    {
        gameController.checkMateEvent += DisplayCheckMate;
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            syncBoardStatus.rivalGiveUp += RivalGiveUp;
        }
    }

    private async void DisplayCheckMate(){
        checkMateUI.SetActive(true);
        await UniTask.Delay(statusDisplayTime)
        .ContinueWith(() => gameResultPanel.SetActive(true)); //ゲームリザルトを表示するまで数秒間をあける
        gameController.checkMateEvent -= DisplayCheckMate;
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            syncBoardStatus.rivalGiveUp += RivalGiveUp;
        }
    }

    private async void RivalGiveUp(){
        giveUpUI.SetActive(true);
        await UniTask.Delay(statusDisplayTime)
        .ContinueWith(() => gameResultPanel.SetActive(true));
        gameController.checkMateEvent -= DisplayCheckMate;
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            syncBoardStatus.rivalGiveUp += RivalGiveUp;
        }
    }

}
