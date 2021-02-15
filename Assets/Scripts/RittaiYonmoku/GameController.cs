using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using CommonConfig;
using System;
public class GameController : MonoBehaviour
{
    public delegate void CheckMateEventHandler();
    public event CheckMateEventHandler checkMateEvent = () => {};
    public delegate void SetUpCompleteEventHandler();
    public event SetUpCompleteEventHandler setUpComplete = () => {};

    [SerializeField] Button goPutButton;
    [SerializeField] Button giveUpButton;
    [SerializeField] BoardController boardController;
    [SerializeField] TimeCountPanel timeCount;
    [SerializeField] GameObject connectFailedPanel;
    [SerializeField] GameObject npc;
    [SerializeField] GameObject connectFirebase;
    [SerializeField] GameObject syncboard;
    [SerializeField] UserNamePanel userNamePanel;
    [SerializeField] Text connectingText;
    [SerializeField] GameObject playBGM;
    private int goNumber = GameRule.TotalGoNumber;
    public int GoNumber{get {return goNumber;}}
    private int currentTurn;
    public int CurrentTurn{
        set {currentTurn = value;}
        get {return currentTurn;}}
    private int playMode;
    public int PlayMode{
        set {playMode = value;}
        get {return playMode;}
    }
    private int gameRoom;
    public int GameRoom{
        set {gameRoom = value;}
        get {return gameRoom;}
    }

    private int player;
    public int Player{
        set {player = value;}
        get {return player;}
    }

    private int rival;
    public int Rival{
        set {rival = value;}
        get {return rival;}
    }

    private string rivalName = "";
    public string RivalName{get {return rivalName;}}

    private async void Start(){
        await SetUpGame();
        userNamePanel.SetPlayerName(player, rivalName);
        boardController.boardUpdated += TurnChange;
        TurnSet(GameRule.FirstAttack); //初ターンセット
        playBGM.SetActive(true);
        timeCount.DoTimeCount = true;
    }

    private async UniTask SetUpGame(){
        if(playMode == GameRule.SoloPlayMode){
            connectingText.enabled = false;
            connectFirebase.SetActive(false);
            syncboard.SetActive(false);
            npc.SetActive(true);
            rivalName = "NPC";
        }
        else{
            npc.SetActive(false);
            connectingText.enabled = true;
            await connectFirebase.GetComponent<ConnectFirebase>().SetGameRoom(gameRoom, player);
            try{
                rivalName = await connectFirebase.GetComponent<ConnectFirebase>().GetRivalName(gameRoom, rival);
            }
            catch (TimeoutException){
                connectFailedPanel.SetActive(true);
            }
            connectFirebase.SetActive(true);
            syncboard.SetActive(true);
            connectingText.enabled = false;
            setUpComplete();
        }
    }

    private void TurnChange(){
        if(!ConfirmCheckMate()){
            goNumber -= 1;
            if(goNumber % 2 == 0){
                //碁の数が偶数の場合は先行の白の手番
                TurnSet(GameRule.FirstAttack);
            }
            else{
                //碁の数が奇数の場合は後攻の黒の手番
                TurnSet(GameRule.SecondAttack);
            }
        };
    }

    private void TurnSet(int nextTurn){
        currentTurn = nextTurn;
        if(currentTurn == player){
            //プレイヤーのターンであれば碁を置く操作とギブアップする操作をできるようにする
            goPutButton.enabled = true;
            giveUpButton.enabled = true;
        }
        else{
            //対戦相手のターンであればギブアップできなくする
            giveUpButton.enabled = false;
        }
    }

    private bool ConfirmCheckMate(){
        //event handlerへ登録する方式だとTurnChangeとの実行順が保証できず
        //先にTurnChangeでcurrentTurnが更新されてしまう恐れがあるため
        //TurnChangeの中でチェックメイトの判定を行い、続く処理を分岐させるようにする
        GoSituations[] checkMateArray = boardController.HasLines(4);
        if(checkMateArray != null){
            timeCount.SwitchTimeCountStatus(false);
            boardController.boardUpdated -= TurnChange; //チェックメイトが発生した場合はターンの切替を行わないようにする
            npc.SetActive(false); //currentTurnが相手の状態なので、余計な碁が置かれないようdisactiveにする
            checkMateEvent();
            return true;
        }
        else{
            return false;
        }
    }
}
