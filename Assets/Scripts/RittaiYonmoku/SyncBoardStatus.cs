using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using Cysharp.Threading.Tasks;
using CustomException;

public class SyncBoardStatus : MonoBehaviour
{
    //相手が降参したことを通知するイベント
    public delegate void RivalGiveUpEventHandler(string text);
    public event RivalGiveUpEventHandler rivalGiveUp = (string text) => {}; 

    [SerializeField] BoardController boardController;
    [SerializeField] GameController gameController;
    [SerializeField] FirebaseUpdateBoardFunc firebaseUpdateBoard;
    [SerializeField] TimeCountPanel timeCountPanel;
    [SerializeField] Text connectingText;
    [SerializeField] GameObject connectFailedPanel;
    private string giveUpText = "相手が降参しました";
    void Start(){
        gameController.setUpComplete += InitializeSyncStatus;
    }

    private void InitializeSyncStatus(){
        if(gameController.Player == GameRule.FirstAttack){
            boardController.boardUpdated += SyncBoard;
        }
        else{
            ListenRival();
        }
        gameController.setUpComplete -= InitializeSyncStatus;
    }

    private void SyncBoard(){
        SetGo();
        ListenRival();
    }

    private void SetGo(){
        connectingText.enabled = true;
        try{
            UniTask.Create(async () => {
                await firebaseUpdateBoard.SetGo(boardController.LastUpdate);
                boardController.boardUpdated -= SyncBoard; //相手のボード更新時に呼び出せれないようにする
            }).Forget();            
        }
        catch{
            connectFailedPanel.SetActive(true);
        }
        finally{
            connectingText.enabled = false;
        }
    }

    private async void ListenRival(){
        connectingText.enabled = true;
        try{
            await UniTask.Run(async () => {
                (int x, int z, int y, int color) rivalAction = await firebaseUpdateBoard.WaitRivalAction();
                boardController.AddGo(rivalAction.x, rivalAction.z, rivalAction.color);
                boardController.boardUpdated += SyncBoard;
            });
        }
        catch (GiveUpSignalReceive){
            //相手がギブアップした場合の処理
            gameController.CurrentTurn = gameController.Player;
            boardController.boardUpdated -= SyncBoard;
            timeCountPanel.SwitchTimeCountStatus(false);
            rivalGiveUp(giveUpText);
        }
        catch{
            connectFailedPanel.SetActive(true);
        }
        finally{
            connectingText.enabled = false;
        }
    }

    public void GiveUpAction(){
        //自身が降参した時の処理
        //Firebaseに降参用のゲームステータスをセットする
        connectingText.enabled = true;
        try{
            UniTask.Create(async () => {
                boardController.boardUpdated -= SyncBoard;
                await firebaseUpdateBoard.SetGo((GameRule.GiveUpSignal, GameRule.GiveUpSignal, GameRule.GiveUpSignal, GameRule.GiveUpSignal));
            }).Forget();
        }
        catch{}
        finally{
            connectingText.enabled = false;
        }
    }
}
