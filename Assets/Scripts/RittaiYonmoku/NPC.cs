using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CommonConfig;

public class NPC : MonoBehaviour
{
    [SerializeField] BoardController boardController;
    [SerializeField] GameController gameController;
    private int myColor;
    private int rivalColor;
    private bool thinking = false;
    private int level = NPCLevel.EasyLevel;
    private NPCBehavior nPCBehavior;

    void Start(){
        if(gameController.Rival == GameRule.FirstAttack){
            myColor = BoardStatus.GoWhite;
            rivalColor = BoardStatus.GoBlack;
        }
        else{
            myColor = BoardStatus.GoBlack;
            rivalColor = BoardStatus.GoWhite;
        };

        nPCBehavior = new NPCBehavior(level);
    }

    void Update(){
        if(myColor == gameController.CurrentTurn & !thinking){
            thinking = true;
            nPCBehavior.EarlyActionEasy(boardController, myColor, rivalColor);
            thinking = false;
        }
    }
}

public class NPCBehavior
{
    public delegate void NPCAction(BoardController boardController, int myColor, int rivalColor);
    public NPCAction earlyAction = (BoardController boardController, int myColor, int rivalColor) => {};
    public NPCAction middleAction = (BoardController boardController, int myColor, int rivalColor) => {};
    public NPCAction lateAction = ( BoardController boardController, int myColor, int rivalColor) => {};

    private CommonBehavior commonBehavior = new CommonBehavior();

    public NPCBehavior(int level){
        if(level == NPCLevel.EasyLevel){
            earlyAction += EarlyActionEasy;
        }
    }

    public void EarlyActionEasy(BoardController boardController, int myColor, int rivalColor){
        (int x, int z)? candidatePos = null;
        Situations[] reachLines = boardController.HasLines(3);
        if(reachLines != null){
            candidatePos = commonBehavior.PreventReach(reachLines.Where(item => item.BoardStatus == rivalColor).ToArray(), boardController);
        }
        if(candidatePos == null){
            candidatePos = commonBehavior.NoPlan(boardController.VacantPos());
        }
        boardController.AddGo(candidatePos.Value.x, candidatePos.Value.z, myColor);
    }
}

public class CommonBehavior
{
        public (int x, int z)? NoPlan(Situations[] vacantArray){
        int rndIndex = Random.Range(0, vacantArray.Length);
        return (vacantArray[0].Positions[rndIndex].x, vacantArray[0].Positions[rndIndex].z);
    }

    public (int x, int z)? PreventReach(Situations[] reachLines, BoardController boardController){
        Situations criticalReachLine = reachLines.Where(item => item.CheckMatePos.Value.y == boardController.CheckCanPut(item.CheckMatePos.Value.x, item.CheckMatePos.Value.z)).FirstOrDefault();
        //Debug.Log(criticalReachLine.CheckMatePos);
        //Debug.Log(boardController.CheckCanPut(criticalReachLine.CheckMatePos.Value.x, criticalReachLine.CheckMatePos.Value.z));
        if(criticalReachLine != null){
            return (criticalReachLine.CheckMatePos.Value.x, criticalReachLine.CheckMatePos.Value.z);
        }
        else{
            //次の手でチェックメイトとなる座標がなかった場合はnull
            return null;
        }
    }
}

public class EarlyBehavior
{

}

public class MiddleBehavior
{

}

public class LateBehavior
{

}