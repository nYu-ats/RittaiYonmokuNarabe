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
            await UniTask.Delay(1500); //碁を置くのが速すぎると、1手前に置いた碁と盤上で衝突してしまうため少し待機する
            if(phaseJudgePoint.Any(item => boardController.CheckCanPut(item.x, item.z) <= item.limit 
                & boardController.CheckCanPut(item.x, item.z) != BoardStatus.CanNotPut)){
                    //主要な位置がすべて埋まっていない間、序盤と判断する
                nPCBehavior.earlyAction(boardController, myColor, rivalColor);
            }
            else if(boardController.VacantPos().Length > 32){
                 //場に出ている碁の数が所定の個数を超えるまでを中盤と判断する
                nPCBehavior.middleAction(boardController, myColor, rivalColor);
            }
            else{
                nPCBehavior.lateAction(boardController, myColor, rivalColor);
            }
            thinking = false;
        }
    }
}

//序盤/中盤/終盤/局面に関係ない共通処理を組み合わせてNPCの挙動を作る
//組み合わせ方によりNPCのレベルを調整するようにする
public class NPCBehavior
{
    public delegate void NPCAction(BoardController boardController, int myColor, int rivalColor);
    public NPCAction earlyAction = (BoardController boardController, int myColor, int rivalColor) => {};
    public NPCAction middleAction = (BoardController boardController, int myColor, int rivalColor) => {};
    public NPCAction lateAction = (BoardController boardController, int myColor, int rivalColor) => {};

    private CommonBehavior commonBehavior = new CommonBehavior();
    private EarlyBehavior earlyBehavior = new EarlyBehavior();
    private MiddleBehavior middleBehavior = new MiddleBehavior();

    public NPCBehavior(int level){
        //NPCのレベル設定も追加したいので受け取った引数に応じて
        //コンストラクタで振る舞いを切り替えられるようにする
        if(level == NPCLevel.EasyLevel){
            earlyAction += EarlyActionEasy;
            middleAction += MiddleActionEasy;
            lateAction += LateActionEasy;
        }
    }

    public void EarlyActionEasy(BoardController boardController, int myColor, int rivalColor){
        //リーチラインの妨害を最優先とし、それ以外はゲームにおいて要となるポジションを埋めに行く
        (int x, int z)? candidatePos = null;
        GoSituations[] reachLines = boardController.HasLines(3);

        if(reachLines != null){
            GoSituations[] myReachLines = reachLines.Where(item => item.BoardStatus == myColor).ToArray();
            GoSituations[] rivalReachLines = reachLines.Where(item => item.BoardStatus == rivalColor).ToArray();
            if(myReachLines.Length > 0){
                //自身のリーチラインに碁を置ける場合は最優先でチェックメイトしにいく
                candidatePos = commonBehavior.PutReachLine(myReachLines, boardController);
            }
            if(candidatePos == null & rivalReachLines.Length > 0){
                //相手のリーチラインがあればそのラインを止めに行く
                candidatePos = commonBehavior.PutReachLine(rivalReachLines, boardController);
            }
        }

        if(candidatePos == null){
            //特にチェックメイトにからむラインがなければ重要ポジションを埋める
            candidatePos = earlyBehavior.PutImportantPosition(boardController);
        }

        boardController.AddGo(candidatePos.Value.x, candidatePos.Value.z, myColor);
    }

    public void MiddleActionEasy(BoardController boardController, int myColor, int rivalColor){
        (int x, int z)? candidatePos = null;
        GoSituations[] single = boardController.HasLines(1);
        GoSituations[] preReachLines = boardController.HasLines(2);    
        GoSituations[] reachLines = boardController.HasLines(3);

        if(reachLines != null){
            GoSituations[] myReachLines = reachLines.Where(item => item.BoardStatus == myColor).ToArray();
            GoSituations[] rivalReachLines = reachLines.Where(item => item.BoardStatus == rivalColor).ToArray();
            if(myReachLines.Length > 0){
                candidatePos = commonBehavior.PutReachLine(myReachLines, boardController);
            }
            if(candidatePos == null & rivalReachLines.Length > 0){
                candidatePos = commonBehavior.PutReachLine(rivalReachLines, boardController);
            }
        }

        //相手の2連ラインがあればそれを邪魔する
        if(candidatePos == null){
            GoSituations[] rivalPreReachLines = preReachLines.Where(item => item.BoardStatus == rivalColor).ToArray();
            candidatePos = middleBehavior.PutPreReachLine(rivalPreReachLines, boardController);
        }

        //孤立している自身の碁があれば2連を作りに行く
        if(candidatePos == null){
            single = single.Where(item => item.BoardStatus == myColor).ToArray();
            candidatePos = middleBehavior.MakePreReachLine(single, boardController);
        }

        //自身の2連ラインがあればリーチを作りに行く
        if(candidatePos == null){
            GoSituations[] myPreReachLines = preReachLines.Where(item => item.BoardStatus == myColor).ToArray();
            candidatePos = middleBehavior.PutPreReachLine(myPreReachLines, boardController);
        }

        //上記いずれの処理も実行しなかった場合、ランダムな位置に置く
        if(candidatePos == null){
            candidatePos = commonBehavior.NoPlan(boardController.VacantPos());
        }

        boardController.AddGo(candidatePos.Value.x, candidatePos.Value.z, myColor);
    }

    public void LateActionEasy(BoardController boardController, int myColor, int rivalColor){
        (int x, int z)? candidatePos = null;
        GoSituations[] single = boardController.HasLines(1);
        GoSituations[] preReachLines = boardController.HasLines(2);    
        GoSituations[] reachLines = boardController.HasLines(3);

        if(reachLines != null){
            GoSituations[] myReachLines = reachLines.Where(item => item.BoardStatus == myColor).ToArray();
            GoSituations[] rivalReachLines = reachLines.Where(item => item.BoardStatus == rivalColor).ToArray();
            if(myReachLines.Length > 0){
                candidatePos = commonBehavior.PutReachLine(myReachLines, boardController);
            }
            if(candidatePos == null & rivalReachLines.Length > 0){
                candidatePos = commonBehavior.PutReachLine(rivalReachLines, boardController);
            }
        }

        if(candidatePos == null){
            GoSituations[] myPreReachLines = preReachLines.Where(item => item.BoardStatus == myColor).ToArray();
            candidatePos = middleBehavior.PutPreReachLine(myPreReachLines, boardController);
        }

        if(candidatePos == null){
            GoSituations[] rivalPreReachLines = preReachLines.Where(item => item.BoardStatus == rivalColor).ToArray();
            candidatePos = middleBehavior.PutPreReachLine(rivalPreReachLines, boardController);
        }

        if(candidatePos == null){
            single = single.Where(item => item.BoardStatus == myColor).ToArray();
            candidatePos = middleBehavior.MakePreReachLine(single, boardController);
        }

        if(candidatePos == null){
            candidatePos = commonBehavior.NoPlan(boardController.VacantPos());
        }

        boardController.AddGo(candidatePos.Value.x, candidatePos.Value.z, myColor);
    }
}

public class CommonBehavior
{
    public (int x, int z)? NoPlan((int x, int z)[] vacantArray){
        //いい手がない場合はランダムな位置に碁を置く
        int rndIndex = Random.Range(0, vacantArray.Length);
        return (vacantArray[rndIndex].x, vacantArray[rndIndex].z);
    }

    public (int x, int z)? PutReachLine(GoSituations[] reachLines, BoardController boardController){
        //碁が3連になっており、次の手でそのラインが埋まる場合はそこを埋める
        GoSituations criticalReachLine = reachLines.Where(item => item.RestPos[0].y == boardController.CheckCanPut(item.RestPos[0].x, item.RestPos[0].z)).FirstOrDefault();
        if(criticalReachLine != null){
            return (criticalReachLine.RestPos[0].x, criticalReachLine.RestPos[0].z);
        }
        else{
            //次の手でチェックメイトとなる座標がなかった場合はnull
            return null;
        }
    }
}

public class EarlyBehavior
{
    //5リーチにからめる12ポジションを埋めにいく
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
    public (int x, int z)? PutPreReachLine(GoSituations[] preReachLines, BoardController boardController){
        //碁が2連になっており、次の手で3連にできる場合はそこを埋める
        (int x, int z, int y)[] preReachPos = preReachLines.SelectMany(item => item.RestPos).ToArray();
        if(preReachPos != null){
            (int x, int z, int y)[] criticalPreReachPos = preReachPos.Where(item => item.y == boardController.CheckCanPut(item.x, item.z)).ToArray();
            if(criticalPreReachPos.Length != 0){
                int rndIndex = Random.Range(0, criticalPreReachPos.Length);
                return (criticalPreReachPos[rndIndex].x, criticalPreReachPos[rndIndex].z);
            }
            else{
                return null;
            }
        }
        else{
            return null;
        }
    }

    public (int x, int z)? MakePreReachLine(GoSituations[] single, BoardController boardController){
        //同色2連を作りに行く処理
        //単独の碁と空きの位置を比べて次の手を提案する
        (int x, int z)[] vacantXZPositions = boardController.VacantPos();
        (int x, int z, int y)[] vacantPos = vacantXZPositions.Select(item => (x:item.x, z:item.z, y:boardController.CheckCanPut(item.x, item.z))).ToArray();
        (int x, int z, int y)[] candidatePos = single.SelectMany(item => item.RestPos).ToArray();
        candidatePos = candidatePos.Where(item => vacantPos.Contains(item)).ToArray();
        if(candidatePos.Length != 0){
            int rndIndex = Random.Range(0, candidatePos.Length);
            return (candidatePos[rndIndex].x, candidatePos[rndIndex].z);
        }
        else{
            return null;
        }
    }
}