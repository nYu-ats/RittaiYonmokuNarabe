using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
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

interface IHasLines{
    Situations[] HasLines(int checkCount);
}

interface IVacantPos{
    Situations[] VacantPos();
}

public class Situations
{
    //空き/白/黒の各状態の情報を格納するクラス
    private (int x, int z, int y)[] positions;
    public (int x, int z, int y)[] Positions{get {return positions;}}
    private int boardStatus;
    public int BoardStatus{get {return boardStatus;}}
    private int pattern;
    public int Pattern{get {return pattern;}}
    private (int x, int z, int y)? checkMatePos = null;
    public (int x, int z, int y)? CheckMatePos{
        //リーチチェックの時のみしか使わないのでコンストラクタに含めず
        //プロパティからセットを行う
        set {checkMatePos = value;}
        get {return checkMatePos;}}
    public Situations((int x, int z, int y)[] thisPosision, int color, int linePattern){
        positions = thisPosision;
        boardStatus = color;
        pattern = linePattern;
    }
}

public class BoardController : MonoBehaviour, IAddGo, ICheckCanPut, IHasLines, IVacantPos
{
    [SerializeField] Board board;
    [SerializeField] GoGenerator goGenerator;
    public delegate void BoardUpdateEventHandler();
    public BoardUpdateEventHandler boardUpdated = () => {}; 

    public void AddGo(int xIndex, int zIndex, int addColor){
        int canPutIndexY = CheckCanPut(xIndex, zIndex);
        if(canPutIndexY != BoardStatus.CanNotPut){
            board.boardStatusArray[Array.IndexOf(board.posArray, (xIndex, zIndex, canPutIndexY))] = addColor;
            goGenerator.PutGo(xIndex, zIndex, addColor);
            boardUpdated();
        }
    }

    public int CheckCanPut(int xIndex, int zIndex){
        //指定のXZ座標で碁が置けるかどうかの確認
        try{
            int[] indexArray = board.posArray.Select((item, index) => new {Index = index, Value = item})
            .Where(item => item.Value.x == xIndex & item.Value.z == zIndex).Select(item => item.Index).ToArray();
            return indexArray.Where(item => board.boardStatusArray[item] == BoardStatus.Vacant)
            .Select(item => board.posArray[item].y).First(); //1次元に平坦化したボード状態配列boardStatusArrayのインデックスを返す
        }
        catch{
            //すでに4つの碁が置かれている場合
            return BoardStatus.CanNotPut;
        }
    }

    public Situations[] VacantPos(){
        (int x, int z, int y)[] vacantArray = board.posArray.Select((item, index) => new {Index = index, Value = item})
        .Where(item => board.boardStatusArray[item.Index] == BoardStatus.Vacant).Select(item => item.Value).ToArray();
        Situations[] vacantPos = new Situations[vacantArray.Length];
        for(int index = 0; index < vacantArray.Length; index++){
            Situations tmpReachLine = new Situations(vacantArray, BoardStatus.Vacant, LinePattern.Pattern0);
            vacantPos[index] = tmpReachLine;
        }
        return vacantPos;
    }

    public Situations[] HasLines(int checkCount){
        try{
            //黒のラインチェック
            //黒の碁が置かれている座標を取得
            (int x, int z, int y)[] blackCandidatehXY = 
            board.posArray.Select((item, index) => new {Index = index, Value = item})
            .Where(item => board.boardStatusArray[item.Index] == BoardStatus.GoBlack)
            .Select(item => item.Value).ToArray();
            //絞り込んだ座標の中から指定の数の碁が並んでいるラインを抽出する
            Situations[] blackReachLines = SearchLines(blackCandidatehXY, BoardStatus.GoBlack, checkCount);

            //白のラインチェック
            (int x, int z, int y)[] whiteCandidatehXY = 
            board.posArray.Select((item, index) => new {Index = index, Value = item})
            .Where(item => board.boardStatusArray[item.Index] == BoardStatus.GoWhite)
            .Select(item => item.Value).ToArray();
            Situations[] whiteReachLines = SearchLines(whiteCandidatehXY, BoardStatus.GoWhite, checkCount);

            //白と黒のラインを結合
            Situations[] allReachLines = blackReachLines.Concat(whiteReachLines).ToArray();
            if(allReachLines.Length > 0){
                //1つ以上のラインがあった場合は配列を返す
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

    private Situations[] SearchLines((int x, int z, int y)[] candidatePos, int color, int checkCount){
        //リーチになりうる条件を元に、既定数連続して置かれている碁の座標を探る
        try{
            Situations[] reachLines = new Situations[0]{};
            //条件1 : XとY座標が変化しないライン
            (int x, int z, int y)[] reachXY = candidatePos.GroupBy(item => new { PosX = item.x, PosY = item.y})
            .Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachXY, color, checkCount, LinePattern.Pattern1)).ToArray(); //Concat使う場合、代入が必要なので注意

            //条件2 : ZとY座標が変化しないライン
            (int x, int z, int y)[] reachZY = candidatePos.GroupBy(item => new { PosZ = item.z, PosY = item.y})
            .Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachZY, color, checkCount, LinePattern.Pattern2)).ToArray();

            //条件3 : XとZ座標が同じライン
            (int x, int z, int y)[] reachXZ = candidatePos.GroupBy(item => new { PosX = item.x, PosZ = item.z})
            .Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachXZ, color, checkCount, LinePattern.Pattern3)).ToArray();

            //条件4 : X座標が変化せずYとZ座標が異なる(1ずつ上昇もしくは下降する)
            //Z正方向に上昇するパターン
            (int x, int z, int y)[] reachDiagonalX1 = candidatePos.Where(item => item.y == item.z)
            .GroupBy(item => item.x).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            //Z正方向に下降するパターン
            (int x, int z, int y)[] reachDiagonalX2 = candidatePos.Where(item => (3 - item.y) == item.z)
            .GroupBy(item => item.x).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalX1, color, checkCount, LinePattern.Pattern4)).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalX2, color, checkCount, LinePattern.Pattern4)).ToArray();

            //条件5 : Z座標が変化せずXとY座標が異なる(1ずつ上昇もしくは下降する)
            //X正方向に上昇するパターン
            (int x, int z, int y)[] reachDiagonalZ1 = candidatePos.Where(item => item.x == item.y)
            .GroupBy(item => item.z).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            //X正方向に下降するパターン
            (int x, int z, int y)[] reachDiagonalZ2 = candidatePos.Where(item => (3 - item.y) == item.x)
            .GroupBy(item => item.z).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalZ1, color, checkCount, LinePattern.Pattern5)).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalZ2, color, checkCount, LinePattern.Pattern5)).ToArray();

            //条件6 : Y座標が変化せずXとZ座標が異なる(1ずつ上昇もしくは下降する)
            //XZ平面で右肩上がり
            (int x, int z, int y)[] reachDiagonalY1 = candidatePos.Where(item => item.x == item.z)
            .GroupBy(item => item.y).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            //XZ平面で右肩下がり
            (int x, int z, int y)[] reachDiagonalY2 = candidatePos.Where(item => (3 - item.z) == item.x)
            .GroupBy(item => item.y).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalY1, color, checkCount, LinePattern.Pattern6)).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalY2, color, checkCount, LinePattern.Pattern6)).ToArray();

            //条件7 : X,Y,Z座標いずれも異なる(1ずつ上昇もしくは下降する)
            //この条件に限っては、1ラインずつ探っていくのでWhere内でのCountでの絞り込みは必要ない
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
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonal1, color, checkCount, LinePattern.Pattern7)).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonal2, color, checkCount, LinePattern.Pattern7)).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonal3, color, checkCount, LinePattern.Pattern7)).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonal4, color, checkCount, LinePattern.Pattern7)).ToArray();

            return reachLines;
        }
        catch{
            return null;
        }
    }

    private Situations[] MakeLineArray((int x, int z, int y)[] candidatePos, int color, int checkCount, int linePatten){
        if(candidatePos.Length % checkCount == 0){
            //指定されている碁が連続している数の倍数分だけ候補がある時のみSituations配列を生成する
            Situations[] goSituations = new Situations[candidatePos.Length / checkCount];
            for(int index = 0; index < candidatePos.Length / checkCount; index++){
                Situations tmpGoSituation = new Situations(candidatePos.Skip(index * checkCount).Take(checkCount).ToArray(), color, linePatten); //リーチラインの切り出し
                if(checkCount == 3){
                    //3連(リーチ)チェックの場合のみチェックメイトになるうる座標の設定処理をする
                    SetCheckMatePos(tmpGoSituation);
                }
                goSituations[index] = tmpGoSituation;
            }
            return goSituations;
        }
        else{
            //定されている碁が連続している数の倍数分だけ候補がなかった場合は、空の配列を返す
            return new Situations[0]{};
        }
    }

    private void SetCheckMatePos(Situations threeGoSituation){
        //LinePatterの値を定数として取得できず、switchが使えないためif文で書く
        int[] lineIndex = new int[]{0, 1, 2, 3};
        if(threeGoSituation.Pattern == LinePattern.Pattern1){
            //X座標は固定なラインなのでZ座標を0-3まで移動させながらY座標の状態を調査する
            int thisIndexX = threeGoSituation.Positions[0].x;
            int thisIndexY = threeGoSituation.Positions[0].y;
            int? tmpIndexZ = lineIndex.Select((index, item) => new {IndexY = CheckCanPut(thisIndexX, index), IndexZ = item})
            .Where(item => item.IndexY <= thisIndexY & item.IndexY != BoardStatus.CanNotPut)
            .Select(item => item.IndexZ).FirstOrDefault(); //対象ラインの中にチェックメイトになりうる空きポジションがなければnullを返す
            if(tmpIndexZ != null){
                threeGoSituation.CheckMatePos = (x: thisIndexX, z: (int)tmpIndexZ, y: thisIndexY);
            }
        }else if(threeGoSituation.Pattern == LinePattern.Pattern2){
            //Z座標は固定なラインなのでX座標を0-3まで移動させながらY座標の状態を調査する
            int thisIndexZ = threeGoSituation.Positions[0].z;
            int thisIndexY = threeGoSituation.Positions[0].y;
            int? tmpIndexX = lineIndex.Select((index, item) => new {IndexY = CheckCanPut(index, thisIndexZ), IndexX = item})
            .Where(item => item.IndexY <= thisIndexY & item.IndexY != BoardStatus.CanNotPut)
            .Select(item => item.IndexX).FirstOrDefault(); //対象ラインの中にチェックメイトになりうる空きポジションがなければnullを返す
            if(tmpIndexX != null){
                threeGoSituation.CheckMatePos = (x: (int)tmpIndexX, z: thisIndexZ, y: thisIndexY);
            }
        }else if(threeGoSituation.Pattern == LinePattern.Pattern3){
            //XとZが固定なのでYの空座標を取得するのみで碁が置けないという状況(null)も発生しない
            int thisIndexX = threeGoSituation.Positions[0].x;
            int thisIndexZ = threeGoSituation.Positions[0].z;
            int tmpIndexY = CheckCanPut(thisIndexX, thisIndexZ);
            threeGoSituation.CheckMatePos = (x: thisIndexX, z:thisIndexZ, y: tmpIndexY);
        }else if(threeGoSituation.Pattern == LinePattern.Pattern4){
            //3連の碁の座標が持っていないZとY座標を抽出、碁が置けるかどうか確認する
            int thisIndexX = threeGoSituation.Positions[0].x;
            int[] hasthisIndexY = threeGoSituation.Positions.Select(item => item.y).ToArray();
            int[] hasthisIndexZ = threeGoSituation.Positions.Select(item => item.z).ToArray();
            int? notHaveIndexZ = lineIndex.Where(item => !hasthisIndexZ.Contains(item)).Select(item => item).FirstOrDefault();
            int? notHaveIndexY = lineIndex.Where(item => !hasthisIndexY.Contains(item)).Select(item => item).FirstOrDefault();
            if(notHaveIndexY != null & notHaveIndexZ != null){
                int canPutIndexY = CheckCanPut(thisIndexX, (int)notHaveIndexZ);
                if(canPutIndexY != BoardStatus.CanNotPut | canPutIndexY <= notHaveIndexY){
                    threeGoSituation.CheckMatePos = (x: thisIndexX, z: (int)notHaveIndexZ, y: (int)notHaveIndexY);
                    }
                }
        }else if(threeGoSituation.Pattern == LinePattern.Pattern5){
            //3連の碁の座標が持っていないXとY座標を抽出、碁が置けるかどうか確認する
            int thisIndexZ = threeGoSituation.Positions[0].z;
            int[] hasthisIndexY = threeGoSituation.Positions.Select(item => item.y).ToArray();
            int[] hasthisIndexX = threeGoSituation.Positions.Select(item => item.x).ToArray();
            int? notHaveIndexY = lineIndex.Where(item => !hasthisIndexY.Contains(item)).Select(item => item).FirstOrDefault();
            int? notHaveIndexX = lineIndex.Where(item => !hasthisIndexX.Contains(item)).Select(item => item).FirstOrDefault();
            if(notHaveIndexY != null & notHaveIndexX != null){
                int canPutIndexY = CheckCanPut((int)notHaveIndexX, thisIndexZ);
                if(canPutIndexY != BoardStatus.CanNotPut | canPutIndexY <= notHaveIndexY){
                    threeGoSituation.CheckMatePos = (x: (int)notHaveIndexX, z: thisIndexZ, y: (int)notHaveIndexY);
                    }
                }
        }else if(threeGoSituation.Pattern == LinePattern.Pattern6){
            //3連の碁の座標が持っていないXとZ座標を抽出、碁が置けるかどうか確認する
            int thisIndexY = threeGoSituation.Positions[0].y;
            int[] hasthisIndexX = threeGoSituation.Positions.Select(item => item.x).ToArray();
            int[] hasthisIndexZ = threeGoSituation.Positions.Select(item => item.z).ToArray();
            int? notHaveIndexX = lineIndex.Where(item => !hasthisIndexX.Contains(item)).Select(item => item).FirstOrDefault();
            int? notHaveIndexZ = lineIndex.Where(item => !hasthisIndexZ.Contains(item)).Select(item => item).FirstOrDefault();
            if(notHaveIndexX != null & notHaveIndexZ != null){
                int canPutIndexY = CheckCanPut((int)notHaveIndexX, (int)notHaveIndexZ);
                if(canPutIndexY != BoardStatus.CanNotPut | canPutIndexY <= thisIndexY){
                    threeGoSituation.CheckMatePos = (x: (int)notHaveIndexX, z: (int)notHaveIndexZ, y: thisIndexY);
                    }
                } 
        }else if(threeGoSituation.Pattern == LinePattern.Pattern7){
            //3連の碁の座標が持っていないX、Y、Z座標を抽出、碁が置けるかどうか確認する
            int[] hasthisIndexX = threeGoSituation.Positions.Select(item => item.x).ToArray();
            int[] hasthisIndexZ = threeGoSituation.Positions.Select(item => item.z).ToArray();
            int[] hasthisIndexY = threeGoSituation.Positions.Select(item => item.y).ToArray();
            int? notHaveIndexX = lineIndex.Where(item => !hasthisIndexX.Contains(item)).Select(item => item).FirstOrDefault();
            int? notHaveIndexZ = lineIndex.Where(item => !hasthisIndexZ.Contains(item)).Select(item => item).FirstOrDefault();
            int? notHaveIndexY = lineIndex.Where(item => !hasthisIndexY.Contains(item)).Select(item => item).FirstOrDefault();
            if(notHaveIndexX != null & notHaveIndexZ != null & notHaveIndexY != null){
                int canPutIndexY = CheckCanPut((int)notHaveIndexX, (int)notHaveIndexZ);
                if(canPutIndexY != BoardStatus.CanNotPut |canPutIndexY <= (int)notHaveIndexY){
                    threeGoSituation.CheckMatePos = (x: (int)notHaveIndexX, z: (int)notHaveIndexZ, y: (int)notHaveIndexY);
                    Debug.Log(threeGoSituation.CheckMatePos);
                    }
                }
        }
    }
}