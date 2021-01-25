using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class GameController : MonoBehaviour
{
    [SerializeField] Button goPutButton;
    [SerializeField] Button giveUpButton;
    [SerializeField] Board board;
    private int goNumber = GameRule.TotalGoNumber;
    private string currentTurn;
    public string CurrentTurn{get {return currentTurn;}}
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

    private string player;
    public string Player{
        set {player = value;}
        get {return player;}
    }

    private string rival;
    public string Rival{
        set {rival = value;}
        get {return rival;}
    }

    private void Start(){
        board.boardUpdated += TurnChange;
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

    private void TurnSet(string nextTurn){
        currentTurn = nextTurn;
        if(currentTurn == player){
            //プレイヤーのターンであれば碁を置く操作とギブアップする操作をできるようにする
            goPutButton.enabled = true;
            giveUpButton.enabled = true;
        }
        else{
            //対戦相手のターンであれば碁を置けなくしてギブアップもできなくする
            goPutButton.enabled = false;
            giveUpButton.enabled = false;
        }
    }
}
