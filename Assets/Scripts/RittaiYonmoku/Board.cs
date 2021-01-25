using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using CommonConfig;

interface IAddGo{
    //碁を追加するためのメソッド
    //Y軸について碁が積み上げられていくだけなので、碁を追加する際はX及びZのインデックスのみを指定
    void AddGo(int xIndex, int zIndex, int addColor);
}

interface ICheckCanPut{
    //ある位置の棒に碁を置けるか否かを確認するメソッド
    int CheckCanPut(int xIndex, int zIndex);
}

interface IHasReachLine{
    ReachLine[] HasReachLine();
}

public class ReachLine
{
    //配列だと外部から扱いずらいためリーチラインクラスを作成
    //リーチが発生した際のエフェクト再生及びNPCでリーチを防ぐ動きをさせる目的でリーチチェックを行うため
    //リーチラインの始点と終点のみ分かればよい
    public int reachColor;
    public (int x, int y, int z) lineStartPos;
    public (int x, int y, int z) lineEndPos;
    public ReachLine((int x, int y, int z)[] reachPos, int color){
        lineStartPos.x = reachPos[0].x;
        lineStartPos.y = reachPos[0].y;
        lineStartPos.z = reachPos[0].z;
        lineEndPos.x = reachPos[2].x;
        lineEndPos.y = reachPos[2].y;
        lineEndPos.z = reachPos[2].z;
        reachColor = color;
    }
}
public class Board : MonoBehaviour, IAddGo, ICheckCanPut, IHasReachLine
{
    //4×4×4のボードを表す
    //配列の切り出しが行いやすく、LInqで扱えるためジャグ配列を使う
    //碁が置かれていない状態として0で初期化
    private int[][][] boardArray = new int[4][][]{
        new int[4][]{
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0}
        },
        new int[4][]{
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0}
        },
        new int[4][]{
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0}
        },
        new int[4][]{
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0},
            new int[4]{0, 0, 0, 0}
        }
    };
    [SerializeField] GoGenerator goGenerator;

    public delegate void BoardUpdateEventHandler();
    public BoardUpdateEventHandler boardUpdated = () => {};

    //データの持ち方変更テスト用
    private (int x, int z, int y)[] posArray = new (int x, int z, int y)[]{};
    private int[] boardStatusArray = Enumerable.Repeat(0, GameRule.TotalGoNumber).ToArray();

    void Awake(){
        //ボードの座標配列の初期化
        //いい方法が思いつかないため一旦forで行う
        for(int indexX = 0; indexX < 4; indexX ++){
            for(int indexZ = 0; indexZ < 4; indexZ++){
                for(int indexY = 0; indexY < 4; indexY++){
                    Array.Resize(ref posArray, posArray.Length + 1);
                    posArray[posArray.Length - 1] = (x:indexX, z:indexZ, y:indexY);
                }
            }
        }
    }

    public void AddGo(int xIndex, int zIndex, int addColor){
        int canPutIndex = CheckCanPut(xIndex, zIndex);
        if(canPutIndex != BoardStatus.CanNotPut){
            boardStatusArray[canPutIndex] = addColor;
            goGenerator.PutGo(xIndex, zIndex, addColor);
            //Debug.Log(boardStatusArray[canPutIndex]);
            //Debug.Log(posArray[canPutIndex]);
            if(HasReachLine() != null){
                Debug.Log("reach");
            }
            boardUpdated();
        }
    }

    public int CheckCanPut(int xIndex, int zIndex){
        //碁が追加されるY方向のインデックスを取得する
        try{
            //return boardArray[xIndex][zIndex].Select((item, index) => new {Index = index, Value = item})
            //.Where(item => item.Value == BoardStatus.Vacant ).Select(item => item.Index).First();

            int[] indexArray = posArray.Select((item, index) => new {Index = index, Value = item})
            .Where(item => item.Value.x == xIndex & item.Value.z == zIndex).Select(item => item.Index).ToArray();

            return indexArray.Where(item => boardStatusArray[item] == BoardStatus.Vacant)
            .Select(item => item).First();
        }
        catch{
            //すでに4つの碁が置かれている場合
            return BoardStatus.CanNotPut;
        }
    }

    public ReachLine[] HasReachLine(){
        try{
            //黒のリーチラインチェック
            //黒の碁が置かれている座標を取得
            (int x, int z, int y)[] blackCandidatehXY = 
            posArray.Select((item, index) => new {Index = index, Value = item})
            .Where(item => boardStatusArray[item.Index] == BoardStatus.GoBlack)
            .Select(item => item.Value).ToArray();
            //絞り込んだ座標の中からリーチがかかっているラインを抽出して返す
            ReachLine[] blackReachLines = ChkReachLine(blackCandidatehXY, BoardStatus.GoBlack);
            Debug.Log(blackReachLines[0]);

            //白のリーチラインチェック
            (int x, int z, int y)[] whiteCandidatehXY = 
            posArray.Select((item, index) => new {Index = index, Value = item})
            .Where(item => boardStatusArray[item.Index] == BoardStatus.GoWhite)
            .Select(item => item.Value).ToArray();
            ReachLine[] whiteReachLines = ChkReachLine(blackCandidatehXY, BoardStatus.GoWhite);

            //白と黒のリーチラインを結合
            ReachLine[] allReachLines = blackReachLines.Concat(whiteReachLines).ToArray();
            Debug.Log(allReachLines[0]);
            return allReachLines;
        }
        catch{
            return null;
        }
    }

    private ReachLine[] ChkReachLine((int x, int z, int y)[] candidatePos, int color){
        try{
            ReachLine[] reachLines = new ReachLine[0]{};
            //リーチ条件1 : XとY座標が同じライン
            (int x, int z, int y)[] reachXY = candidatePos.GroupBy(item => new { PosX = item.x, PosY = item.y})
            .Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachXY, color)).ToArray();
            return reachLines;
        }
        catch{
            return null;
        }
    }

    private ReachLine[] MakeReachLineArray((int x, int z, int y)[] reachPos, int color){
        if(reachPos.Length % 3 == 0){
            //リーチライン候補が3の倍数個あった場合のみ処理を行う
            ReachLine[] reachLines = new ReachLine[reachPos.Length / 3];
            for(int index = 0; index < reachPos.Length / 3; index++){
                ReachLine tmpReachLine = new ReachLine(reachPos.Skip(index * 3).Take(3).ToArray(), color); //リーチラインの切り出し
                reachLines[index] = tmpReachLine;
            }
            return reachLines;
        }
        else{
            //リーチライン候補が3の倍数来なかった場合は、空の配列を返す
            return new ReachLine[0]{};
        }
    }
}

