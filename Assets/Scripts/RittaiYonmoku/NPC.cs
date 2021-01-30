using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CommonConfig;
using Cysharp.Threading.Tasks;

public class NPC : MonoBehaviour
{
    [SerializeField] BoardController boardController;
    [SerializeField] GameController gameController;
    private int myColor;
    private int rivalColor;
    private bool thinking = false;
    private int level = NPCLevel.EasyLevel;
    private NPCBehavior nPCBehavior;
    private (int x, int z, int limit)[] phaseJudgePoint = GamePhaseJudge.EarlyPhasePoint;

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

    async void Update(){
        if(myColor == gameController.CurrentTurn & !thinking){
            thinking = true;
            await UniTask.Delay(1500); //碁を置くのが速すぎると、1手前に置いた碁と衝突してしまうため少し待機
            if(phaseJudgePoint.Any(item => boardController.CheckCanPut(item.x, item.z) <= item.limit 
                & boardController.CheckCanPut(item.x, item.z) != BoardStatus.CanNotPut)){
                    //主要な位置がすべて埋まっていない間を序盤と判断する
                nPCBehavior.earlyAction(boardController, myColor, rivalColor);
            }
            else if(boardController.VacantPos().Length <=32){
                //場に出ている碁の数が所定の個数を超えるまでを中盤と判断する
            }
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
    private EarlyBehavior earlyBehavior = new EarlyBehavior();

    public NPCBehavior(int level){
        //おいおいNPCのレベル設定も追加したいので受け取った引数に応じて
        //コンストラクタで振る舞いを切り替えるようにする
        if(level == NPCLevel.EasyLevel){
            earlyAction += EarlyActionEasy;
            middleAction += MiddleActionEasy;
        }
    }

    public void EarlyActionEasy(BoardController boardController, int myColor, int rivalColor){
        //リーチラインの妨害を最優先とし、それ以外は主要ポジションを埋めに行く
        (int x, int z)? candidatePos = null;
        Situations[] reachLines = boardController.HasLines(3);
        if(reachLines != null){
            candidatePos = commonBehavior.PreventReach(reachLines.Where(item => item.BoardStatus == rivalColor).ToArray(), boardController);
        }
        if(candidatePos == null){
            candidatePos = earlyBehavior.PutImportantPosition(boardController);
        }
        boardController.AddGo(candidatePos.Value.x, candidatePos.Value.z, myColor);
    }

    public void MiddleActionEasy(BoardController boardController, int myColor, int rivalColor){
        
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
    private (int x, int z, int limit)[] checkPoint = GamePhaseJudge.EarlyPhasePoint;

    public (int x, int z) PutImportantPosition(BoardController boardController){
        //序盤での主要な位置をランダムで埋めていく
        (int x, int z)[] candidatePos = checkPoint.Where(item => boardController.CheckCanPut(item.x, item.z) <= item.limit & boardController.CheckCanPut(item.x, item.z) != BoardStatus.CanNotPut)
        .Select(item => (item.x, item.z)).ToArray();
        int rndIndex = Random.Range(0, candidatePos.Length);
        return candidatePos[rndIndex];
    }
}

public class MiddleBehavior
{

}

public class LateBehavior
{

}