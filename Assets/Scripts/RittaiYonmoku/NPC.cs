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
    private bool thinking = false;
    private int level = NPCLevel.EasyLevel;
    private NPCBehavior nPCBehavior;

    void Start(){
        if(gameController.Rival == GameRule.FirstAttack){
            myColor = BoardStatus.GoWhite;
        }
        else{
            myColor = BoardStatus.GoBlack;
        };

        nPCBehavior = new NPCBehavior(level);
    }

    void Update(){
        if(myColor == gameController.CurrentTurn & !thinking){
            thinking = true;
            nPCBehavior.EarlyActionEasy(boardController, myColor);
            thinking = false;
        }
    }
}

public class NPCBehavior
{
    public delegate void NPCAction(BoardController boardController, int myColor);
    public NPCAction earlyAction = (BoardController boardController, int myColor) => {};
    public NPCAction middleAction = (BoardController boardController, int myColor) => {};
    public NPCAction lateAction = ( BoardController boardController, int myColor) => {};

    private CommonBehavior commonBehavior = new CommonBehavior();

    public NPCBehavior(int level){
        if(level == NPCLevel.EasyLevel){
            earlyAction += EarlyActionEasy;
        }
    }

    public void EarlyActionEasy(BoardController boardController, int myColor){
        (int x, int z) sujestPos;
        Situations[] reachLines = boardController.HasLines(3);
        if(reachLines != null){
            //sujestPos = commonBehavior.PreventReach(reachLines.Where(item => item.BoardStatus == myColor).ToArray());
        }
        sujestPos = commonBehavior.NoPlan(boardController.VacantPos());
        if(boardController.CheckCanPut(sujestPos.x, sujestPos.z) != BoardStatus.CanNotPut){
            //そもそも置ける前提なのでvacantチェックはいらない
            boardController.AddGo(sujestPos.x, sujestPos.z, myColor);
        }
    }

}

public class CommonBehavior
{
    public (int x, int z) NoPlan(Situations[] vacantArray){
        int rndIndex = Random.Range(0, vacantArray.Length);
        return (vacantArray[0].Positions[rndIndex].x, vacantArray[0].Positions[rndIndex].z);
    }

    //public (int x, int z) PreventReach(Situations[] reachLines){
    //}
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