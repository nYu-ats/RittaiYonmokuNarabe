using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using CommonConfig;
using System;
public class GameController : MonoBehaviour
{
    [SerializeField] Button goPutButton;
    [SerializeField] Button giveUpButton;
    [SerializeField] BoardController boardController;
    [SerializeField] TimeCountPanel timeCount;
    [SerializeField] GameObject connectFailedPanel;
    [SerializeField] GameObject npc;
    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] UserNamePanel userNamePanel;
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

    public delegate void CheckMateEventHandler();
    public event CheckMateEventHandler checkMateEvent = () => {};
    private async void Start(){
        await SetUpGame();
        userNamePanel.SetPlayerName(player, rivalName);
        boardController.boardUpdated += ConfirmCheckMate;
        boardController.boardUpdated += TurnChange;
        TurnSet(GameRule.FirstAttack); //ゲーム開始時のターンのセット
        timeCount.DoTimeCount = true;
    }

    private async UniTask SetUpGame(){
        if(playMode == 1){
            npc.SetActive(true);
            rivalName = "NPC";
        }
        else{
            npc.SetActive(false);
            await connectFirebase.SetGameRoom(gameRoom, player);
            try{
                rivalName = await connectFirebase.GetRivalName(gameRoom, rival);
            }
            catch (TimeoutException){
                connectFailedPanel.SetActive(true);
            }
        }
    }

    private void TurnChange(){
        goNumber -= 1;
        if(goNumber % 2 == 0){
            //碁の数が偶数の場合は先行の白の手番
            TurnSet(GameRule.FirstAttack);
        }
        else{
            //碁の数が奇数の場合は後攻の黒の手番
            TurnSet(GameRule.SecondAttack);
        }
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

    private void ConfirmCheckMate(){
        GoSituations[] checkMateArray = boardController.HasLines(4);
        if(checkMateArray != null){
            timeCount.SwitchTimeCountStatus(false);
            boardController.boardUpdated -= TurnChange; //チェックメイトが発生した場合はターンの切替を行わないようにする
            checkMateEvent();
        }
    }
}
