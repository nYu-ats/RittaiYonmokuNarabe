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
    public (int x, int y, int z) lineStartPos;
    public (int x, int y, int z) lineEndPos;
    public ReachLine((int x, int y, int z)[] reachPos){
        lineStartPos.x = reachPos[0].x;
        lineStartPos.y = reachPos[0].y;
        lineStartPos.z = reachPos[0].z;
        lineEndPos.x = reachPos[2].x;
        lineEndPos.y = reachPos[2].y;
        lineEndPos.z = reachPos[2].z;
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
        /*int yIndex = CheckCanPut(xIndex, zIndex);
        if(yIndex != BoardStatus.CanNotPut){
            boardArray[xIndex][zIndex][yIndex] = addColor;
            goGenerator.PutGo(xIndex, zIndex, addColor);
            boardUpdated();
        }
        else{
            return;
        }
        */
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
        //黒のリーチラインチェック
        //黒の碁が置かれている座標を取得
        try{
            (int x, int z, int y)[] tmpArray = 
            posArray.Select((item, index) => new {Index = index, Value = item})
            .Where(item => boardStatusArray[item.Index] == BoardStatus.GoBlack)
            .Select(item => item.Value).ToArray();
            //絞り込んだ座標の中からリーチがかかっているラインを抽出して返す
            return ChkReachLine(tmpArray);
        }
        catch{
            return null;
        }

        //白のリーチラインチェック
    }

    private ReachLine[] ChkReachLine((int x, int z, int y)[] candidatePos){
        try{
            ReachLine[] reachLines = new ReachLine[1];
            //リーチ条件1 : XとY座標が同じライン
            (int x, int z, int y)[] reachXY = candidatePos.GroupBy(item => new { PosX = item.x, PosY = item.y})
            .Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            reachLines[0] = new ReachLine(reachXY);
            return reachLines;
        }
        catch{
            return null;
        }
    }
}

