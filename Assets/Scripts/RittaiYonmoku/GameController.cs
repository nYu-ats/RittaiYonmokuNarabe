using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class GameController : MonoBehaviour
{
    [SerializeField] Button goPutButton;
    [SerializeField] Button giveUpButton;
    [SerializeField] BoardController boardController;
    private int goNumber = GameRule.TotalGoNumber;
    public int GoNumber{get {return goNumber;}}
    private int currentTurn;
    public int CurrentTurn{get {return currentTurn;}}
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

    public delegate void CheckMateEventHandler();
    public event CheckMateEventHandler checkMateEvent = () => {};
    private void Start(){
        boardController.boardUpdated += ConfirmCheckMate; //ターンを切り替える前にチェックメイトの確認をする
        boardController.boardUpdated += TurnChange;
        TurnSet(GameRule.FirstAttack); //ゲーム開始時のターンのセット
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
            //対戦相手のターンであれば碁を置けなくしてギブアップもできなくする
            giveUpButton.enabled = false;
        }
    }

    private void ConfirmCheckMate(){
        GoSituations[] checkMateArray = boardController.HasLines(4);
        if(checkMateArray != null){
            boardController.boardUpdated -= TurnChange; //チェックメイトが発生した場合はターンの切替を行わないようにする
            checkMateEvent();
        }
    }
}
