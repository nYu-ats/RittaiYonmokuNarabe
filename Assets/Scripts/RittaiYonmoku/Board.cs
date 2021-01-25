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
    public (int x, int y, int z)[] reachPos;
    public ReachLine((int x, int y, int z)[] posision, int color){
        reachPos = posision;
        reachColor = color;
    }
}
public class Board : MonoBehaviour, IAddGo, ICheckCanPut, IHasReachLine
{
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
            ReachLine[] blackReachLines = SearchReachLine(blackCandidatehXY, BoardStatus.GoBlack);

            //白のリーチラインチェック
            (int x, int z, int y)[] whiteCandidatehXY = 
            posArray.Select((item, index) => new {Index = index, Value = item})
            .Where(item => boardStatusArray[item.Index] == BoardStatus.GoWhite)
            .Select(item => item.Value).ToArray();
            ReachLine[] whiteReachLines = SearchReachLine(whiteCandidatehXY, BoardStatus.GoWhite);

            //白と黒のリーチラインを結合
            ReachLine[] allReachLines = blackReachLines.Concat(whiteReachLines).ToArray();
            if(allReachLines.Length > 0){
                //1つ以上のリーチがある場合は配列を返す
                return allReachLines;
            }
            else{
                return null;
            }
        }
        catch{
            return null;
        }
    }

    private ReachLine[] SearchReachLine((int x, int z, int y)[] candidatePos, int color){
        try{
            ReachLine[] reachLines = new ReachLine[0]{};
            //リーチ条件1 : XとY座標が変化しないライン
            (int x, int z, int y)[] reachXY = candidatePos.GroupBy(item => new { PosX = item.x, PosY = item.y})
            .Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachXY, color)).ToArray(); //Concat使う場合、代入が必要なので注意

            //リーチ条件2 : ZとY座標が変化しないライン
            (int x, int z, int y)[] reachZY = candidatePos.GroupBy(item => new { PosZ = item.z, PosY = item.y})
            .Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachZY, color)).ToArray();

            //リーチ条件3 : XとZ座標が同じライン
            (int x, int z, int y)[] reachXZ = candidatePos.GroupBy(item => new { PosX = item.x, PosZ = item.z})
            .Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachXZ, color)).ToArray();

            //リーチ条件4 : X座標が変化せずYとZ座標が異なる(1ずつ上昇もしくは下降する)
            //Z正方向に上昇するパターン
            (int x, int z, int y)[] reachDiagonalX1 = candidatePos.Where(item => item.y == item.z)
            .GroupBy(item => item.x).Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            //Z正方向に下降するパターン
            (int x, int z, int y)[] reachDiagonalX2 = candidatePos.Where(item => (3 - item.y) == item.z)
            .GroupBy(item => item.x).Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonalX1, color)).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonalX2, color)).ToArray();

            //リーチ条件5 : Z座標が変化せずXとY座標が異なる(1ずつ上昇もしくは下降する)
            //X正方向に上昇するパターン
            (int x, int z, int y)[] reachDiagonalZ1 = candidatePos.Where(item => item.x == item.y)
            .GroupBy(item => item.z).Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            //X正方向に下降するパターン
            (int x, int z, int y)[] reachDiagonalZ2 = candidatePos.Where(item => (3 - item.y) == item.x)
            .GroupBy(item => item.z).Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonalZ1, color)).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonalZ2, color)).ToArray();

            //リーチ条件6 : Y座標が変化せずXとZ座標が異なる(1ずつ上昇もしくは下降する)
            //XZ平面で右肩上がり
            (int x, int z, int y)[] reachDiagonalY1 = candidatePos.Where(item => item.x == item.z)
            .GroupBy(item => item.y).Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            //XZ平面で右肩下がり
            (int x, int z, int y)[] reachDiagonalY2 = candidatePos.Where(item => (3 - item.z) == item.x)
            .GroupBy(item => item.y).Where(item => item.Count() > 2).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonalY1, color)).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonalY2, color)).ToArray();

            //リーチ条件7 : X,Y,Z座標いずれも異なる(1ずつ上昇もしくは下降する)
            //(3, 3, 3)へ向けて上昇
            (int x, int z, int y)[] reachDiagonal1 = candidatePos.Where(item => (item.x == item.y) && (item.z == item.y))
            .Select(item =>item).ToArray();
            //(0, 0, 3)へ向けて上昇
            (int x, int z, int y)[] reachDiagonal2 = candidatePos.Where(item => (item.x == (3 - item.y)) && (item.z == (3 - item.y)))
            .Select(item =>item).ToArray();        
            //(0, 3, 3)へ向けて上昇
            (int x, int z, int y)[] reachDiagonal3 = candidatePos.Where(item => ((3 - item.x) == item.y) && (item.z == item.y))
            .Select(item =>item).ToArray();        
            //(3, 3, 0)へ向けて上昇
            (int x, int z, int y)[] reachDiagonal4 = candidatePos.Where(item => (item.x == item.y) && ((3 - item.z) == item.y))
            .Select(item =>item).ToArray();        
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonal1, color)).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonal2, color)).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonal3, color)).ToArray();
            reachLines = reachLines.Concat(MakeReachLineArray(reachDiagonal4, color)).ToArray();

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

