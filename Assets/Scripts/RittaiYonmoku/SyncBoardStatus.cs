using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using Cysharp.Threading.Tasks;
using CustomException;

public class SyncBoardStatus : MonoBehaviour
{
    //相手が降参したことを通知するイベント
    public delegate void RivalGiveUpEventHandler();
    public event RivalGiveUpEventHandler rivalGiveUp = () => {}; 

    [SerializeField] BoardController boardController;
    [SerializeField] GameController gameController;
    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] TimeCountPanel timeCountPanel;
    [SerializeField] Text connectingText;
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
        UniTask.Create(async () => {
            await connectFirebase.SetGo(boardController.LastUpdate);
            boardController.boardUpdated -= SyncBoard; //相手のボード更新時に呼び出せれないようにする
            connectingText.enabled = false;
        }).Forget();
    }

    private async void ListenRival(){
        connectingText.enabled = true;
        try{
            await UniTask.Run(async () => {
                (int x, int z, int y, int color) rivalAction = await connectFirebase.WaitRivalAction();
                boardController.AddGo(rivalAction.x, rivalAction.z, rivalAction.color);
                boardController.boardUpdated += SyncBoard;
            });
        }
        catch (GiveUpSignalReceive){
            //相手がギブアップした場合の処理
            gameController.CurrentTurn = gameController.Player;
            boardController.boardUpdated -= SyncBoard;
            timeCountPanel.SwitchTimeCountStatus(false);
            rivalGiveUp();
        }
        finally{
            connectingText.enabled = false;
        }
    }

    public void GiveUpAction(){
        //自身が降参した時の処理
        //Firebaseに降参用のゲームステータスをセットする
        connectingText.enabled = true;
        UniTask.Create(async () => {
            boardController.boardUpdated -= SyncBoard;
            await connectFirebase.SetGo((GameRule.GiveUpSignal, GameRule.GiveUpSignal, GameRule.GiveUpSignal, GameRule.GiveUpSignal));
            connectingText.enabled = false;
        }).Forget();
    }
}
