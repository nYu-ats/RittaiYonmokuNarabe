﻿using System;
using System.Linq;
using UnityEngine;
using CommonConfig;
using Cysharp.Threading.Tasks;

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
    //連続して置かれている碁を確認する
    GoSituations[] HasLines(int checkCount);
}

interface IVacantPos{
    //空の座標を確認する
    (int x, int z)[] VacantPos();
}

public class GoSituations
{
    //空き/白/黒の各状態の情報を格納するクラス
    private (int x, int z, int y)[] positions;
    public (int x, int z, int y)[] Positions{get {return positions;}}
    private int boardStatus; //白か黒か空かを格納
    public int BoardStatus{get {return boardStatus;}}
    private int pattern; //ラインのパターンを格納
    public int Pattern{get {return pattern;}}
    //各パターンごとにチェックメイトとなりうる座標がある
    //それを格納する配列
    private (int x, int z, int y)[] restPos = new (int x, int z, int y)[3];
    public (int x, int z, int y)[] RestPos{get {return restPos;}} //参照する時は配列をまとめて取得する
    public (int x, int z, int y) this[int i]{
        set {this.RestPos[i] = value;}
    }
    public GoSituations((int x, int z, int y)[] thisPosision, int color, int linePattern){
        positions = thisPosision;
        boardStatus = color;
        pattern = linePattern;
    }
}

public class BoardController : MonoBehaviour, IAddGo, ICheckCanPut, IHasLines, IVacantPos
{
    public delegate void BoardUpdateEventHandler();
    public event BoardUpdateEventHandler boardUpdated = () => {};
    [SerializeField] Board board;
    [SerializeField] GoGenerator goGenerator;
    [SerializeField] TimeCountPanel timeCountPanel;
    [SerializeField] GameController gameController;
    private (int x, int z, int y, int color) lastUpdate;
    public (int x, int z, int y, int color) LastUpdate{get {return lastUpdate;}}

    void Start(){
        timeCountPanel.timeOut += RandomPut;
    }

    public void AddGo(int xIndex, int zIndex, int addColor){
        int canPutIndexY = CheckCanPut(xIndex, zIndex);
        if(canPutIndexY != BoardStatus.CanNotPut){
            board.boardStatusArray[Array.IndexOf(board.posArray, (xIndex, zIndex, canPutIndexY))] = addColor;
            goGenerator.PutGo(xIndex, zIndex, canPutIndexY, addColor);
            lastUpdate = (xIndex, zIndex, canPutIndexY, addColor);
            boardUpdated();
        }
    }
    private void RandomPut(){
        //制限時間を過ぎた場合には適当な場所に碁を置く
        (int x, int z)[] canPutPos = VacantPos();
        if(canPutPos != null){
            int rndIndex = UnityEngine.Random.Range(0, canPutPos.Length);
            AddGo(canPutPos[rndIndex].x, canPutPos[rndIndex].z, gameController.CurrentTurn);
        }
    }

    public int CheckCanPut(int xIndex, int zIndex){
        //指定のXZ座標で碁が置けるかどうかの確認
        try{
            int[] indexArray = board.posArray.Select((item, index) => new {Index = index, Value = item})
                               .Where(item => item.Value.x == xIndex & item.Value.z == zIndex).Select(item => item.Index).ToArray();

            return indexArray.Where(item => board.boardStatusArray[item] == BoardStatus.Vacant)
                   .Select(item => board.posArray[item].y).First(); //nullだった場合例外スロー
        }
        catch{
            //すでに4つの碁が置かれている場合
            return BoardStatus.CanNotPut;
        }
    }

    public (int x, int z)[] VacantPos(){
        //ボードの空き状況を調べる
        (int x, int z, int y)[] vacantArray = board.posArray.Select((item, index) => new {Index = index, Value = item})
                                              .Where(item => board.boardStatusArray[item.Index] == BoardStatus.Vacant).Select(item => item.Value).ToArray();
        //抜き出した空ポジションから返却用のオブジェクトを作る
        (int x, int z)[] vacantPos = new (int x, int z)[vacantArray.Length];
        for(int index = 0; index < vacantArray.Length; index++){
            vacantPos[index] = (vacantArray[index].x, vacantArray[index].z);
            //vacantPos[index] = tmpReachLine;
        }
        return vacantPos;
    }

    //ラインチェック流れ
    //碁の色で絞り込む->リーチパターンに応じてフィルターをかける->碁の状態オブジェクトを生成
    public GoSituations[] HasLines(int checkCount){
        try{
            //黒のラインチェック
            //黒の碁が置かれている座標を取得
            (int x, int z, int y)[] blackCandidatehXY = board.posArray.Select((item, index) => new {Index = index, Value = item})
            　　　　　　　　　　　　　　　　　　　　　　　　.Where(item => board.boardStatusArray[item.Index] == BoardStatus.GoBlack)
            　　　　　　　　　　　　　　　　　　　　　　　　.Select(item => item.Value).ToArray();
            //絞り込んだ座標の中から指定の数の碁が並んでいるラインを抽出する
            GoSituations[] blackReachLines = SearchLines(blackCandidatehXY, BoardStatus.GoBlack, checkCount);

            //白のラインチェック
            (int x, int z, int y)[] whiteCandidatehXY = board.posArray.Select((item, index) => new {Index = index, Value = item})
            　　　　　　　　　　　　　　　　　　　　　　　　.Where(item => board.boardStatusArray[item.Index] == BoardStatus.GoWhite)
            　　　　　　　　　　　　　　　　　　　　　　　　.Select(item => item.Value).ToArray();
            GoSituations[] whiteReachLines = SearchLines(whiteCandidatehXY, BoardStatus.GoWhite, checkCount);

            //白と黒のラインを結合
            GoSituations[] allReachLines = blackReachLines.Concat(whiteReachLines).ToArray();
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

    private GoSituations[] SearchLines((int x, int z, int y)[] candidatePos, int color, int checkCount){
        //リーチになりうる条件7パターンを元に、指定の数の碁が連続して置かれている座標を探る
        //1 - 7まで順次調査をしながら、条件に合致したら配列を結合していく
        try{
            GoSituations[] reachLines = new GoSituations[0]{};
            //条件1 : XとY座標が変化しないライン
            //XとYでグループ化
            (int x, int z, int y)[] reachXY = candidatePos.GroupBy(item => new { PosX = item.x, PosY = item.y})
            　　　　　　　　　　　　　　　　　　　.Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachXY, color, checkCount, LinePattern.Pattern1)).ToArray(); //Concat使う場合、代入が必要なので注意

            //条件2 : ZとY座標が変化しないライン
            //ZとYでグループ化
            (int x, int z, int y)[] reachZY = candidatePos.GroupBy(item => new { PosZ = item.z, PosY = item.y})
                                              .Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachZY, color, checkCount, LinePattern.Pattern2)).ToArray();

            //条件3 : XとZ座標が同じライン
            //XとZでグループ化
            (int x, int z, int y)[] reachXZ = candidatePos.GroupBy(item => new { PosX = item.x, PosZ = item.z})
                                              .Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachXZ, color, checkCount, LinePattern.Pattern3)).ToArray();

            //条件4 : X座標が変化せずYとZ座標が異なる(1ずつ上昇もしくは下降する)
            //Z正方向に上昇するパターン
            //YとZ座標が同値でフィルターをかけたうえでXでグループ化
            (int x, int z, int y)[] reachDiagonalX1 = candidatePos.Where(item => item.y == item.z)

                                                      .GroupBy(item => item.x).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalX1, color, checkCount, LinePattern.Pattern4)).ToArray();
            //Z正方向に下降するパターン
            //3-YとZが同値でフィルターをかけたうえでXでグループ化
            (int x, int z, int y)[] reachDiagonalX2 = candidatePos.Where(item => (3 - item.y) == item.z)
                                                      .GroupBy(item => item.x).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalX2, color, checkCount, LinePattern.Pattern4)).ToArray();

            //条件5 : Z座標が変化せずXとY座標が異なる(1ずつ上昇もしくは下降する)
            //X正方向に上昇するパターン
            //XとY座標が同値でフィルターをかけたうえでZでグループ化
            (int x, int z, int y)[] reachDiagonalZ1 = candidatePos.Where(item => item.x == item.y)
                                                      .GroupBy(item => item.z).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalZ1, color, checkCount, LinePattern.Pattern5)).ToArray();
            //X正方向に下降するパターン
            //3-YとXが同値でフィルターをかけたうえでZでグループ化
            (int x, int z, int y)[] reachDiagonalZ2 = candidatePos.Where(item => (3 - item.y) == item.x)
                                                      .GroupBy(item => item.z).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalZ2, color, checkCount, LinePattern.Pattern5)).ToArray();

            //条件6 : Y座標が変化せずXとZ座標が異なる(1ずつ上昇もしくは下降する)
            //XZ平面で右肩上がり
            //XとZ座標が同値でフィルターをかけたうえでYでグループ化
            (int x, int z, int y)[] reachDiagonalY1 = candidatePos.Where(item => item.x == item.z)
                                                      .GroupBy(item => item.y).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalY1, color, checkCount, LinePattern.Pattern6)).ToArray();
            //XZ平面で右肩下がり
            //3-ZとXが同値でフィルターをかけたうえでYでグループ化
            (int x, int z, int y)[] reachDiagonalY2 = candidatePos.Where(item => (3 - item.z) == item.x)
                                                      .GroupBy(item => item.y).Where(item => item.Count() == checkCount).SelectMany(item => item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonalY2, color, checkCount, LinePattern.Pattern6)).ToArray();

            //条件7 : X,Y,Z座標いずれも異なる(1ずつ上昇もしくは下降する)
            //この条件に限っては、1ラインずつ探っていくのでWhere内でのCountでの絞り込む必要はない
            //(3, 3, 3)へ向けて上昇
            //X==Y==Zとなる
            (int x, int z, int y)[] reachDiagonal1 = candidatePos.Where(item => (item.x == item.y) && (item.z == item.y))
                                                     .Select(item =>item).ToArray();
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonal1, color, checkCount, LinePattern.Pattern7)).ToArray();
            //(0, 0, 3)へ向けて上昇
            //X==(3-Y)==Zとなる
            (int x, int z, int y)[] reachDiagonal2 = candidatePos.Where(item => (item.x == (3 - item.y)) && (item.z == (3 - item.y)))
                                                     .Select(item =>item).ToArray();        
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonal2, color, checkCount, LinePattern.Pattern7)).ToArray();
            //(0, 3, 3)へ向けて上昇
            //(3-X)==Y==Zとなる
            (int x, int z, int y)[] reachDiagonal3 = candidatePos.Where(item => ((3 - item.x) == item.y) && (item.z == item.y))
                                                     .Select(item =>item).ToArray();        
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonal3, color, checkCount, LinePattern.Pattern7)).ToArray();
            //(3, 3, 0)へ向けて上昇
            //X==Y==(3-Z)となる
            (int x, int z, int y)[] reachDiagonal4 = candidatePos.Where(item => (item.x == item.y) && ((3 - item.z) == item.y))
                                                     .Select(item =>item).ToArray();        
            reachLines = reachLines.Concat(MakeLineArray(reachDiagonal4, color, checkCount, LinePattern.Pattern7)).ToArray();

            return reachLines;
        }
        catch{
            return null;
        }
    }

    private GoSituations[] MakeLineArray((int x, int z, int y)[] candidatePos, int color, int checkCount, int linePatten){
        //指定されている碁が連続している数の倍数分だけ候補がある時のみ配列を生成する
        if(candidatePos.Length % checkCount == 0){
            GoSituations[] goSituations = new GoSituations[candidatePos.Length / checkCount];
            //調査している碁の連続数に応じて排列を切り出す
            for(int index = 0; index < candidatePos.Length / checkCount; index++){
                GoSituations tmpGoSituation = new GoSituations(candidatePos.Skip(index * checkCount).Take(checkCount).ToArray(), color, linePatten); //リーチラインの切り出し
                SetCheckMatePos(tmpGoSituation);
                goSituations[index] = tmpGoSituation;
            }
            return goSituations;
        }
        else{
            //指定の連続数の倍数と配列の長さの対応が誤っていた場合は空の配列を返す
            return new GoSituations[0]{};
        }
    }

    private int[] lineIndex = new int[4]{0, 1, 2, 3};
    private void SetCheckMatePos(GoSituations threeGoSituation){
        //各リーチラインのパターン次応じて、チェックメイトになりうる碁の座標を探る
        //LinePattenの値を定数として取得できず、switchが使えないためif文で書いていく
        if(threeGoSituation.Pattern == LinePattern.Pattern1){
            //X座標は固定なラインなのでZ座標を0-3まで移動させながらY座標の状態を調査する
            int thisIndexX = threeGoSituation.Positions[0].x;
            int thisIndexY = threeGoSituation.Positions[0].y;
            int[] tmpIndexZ = lineIndex.Select((index, item) => new {IndexY = CheckCanPut(thisIndexX, index), IndexZ = item})
                              .Where(item => item.IndexY <= thisIndexY & item.IndexY != BoardStatus.CanNotPut)
                              .Select(item => item.IndexZ).ToArray(); //対象ラインの中にチェックメイトになりうる空きポジションがなければnullを返す
            if(tmpIndexZ != null){
                for(int index = 0; index < tmpIndexZ.Length; index++){
                    threeGoSituation[index] = (x: thisIndexX, z: tmpIndexZ[index], y: thisIndexY);
                }
            }
        }
        else if(threeGoSituation.Pattern == LinePattern.Pattern2){
            //Z座標は固定なラインなのでX座標を0-3まで移動させながらY座標の状態を調査する
            int thisIndexZ = threeGoSituation.Positions[0].z;
            int thisIndexY = threeGoSituation.Positions[0].y;
            int[] tmpIndexX = lineIndex.Select((index, item) => new {IndexY = CheckCanPut(index, thisIndexZ), IndexX = item})
                              .Where(item => item.IndexY <= thisIndexY & item.IndexY != BoardStatus.CanNotPut)
                              .Select(item => item.IndexX).ToArray();
            if(tmpIndexX != null){
                for(int index = 0; index < tmpIndexX.Length; index++){
                    threeGoSituation[index] = (x: tmpIndexX[index], z: thisIndexZ, y: thisIndexY);
                }
            }
        }
        else if(threeGoSituation.Pattern == LinePattern.Pattern3){
            //XとZが固定なのでYの空座標を取得するのみで碁が置けないという状況は発生しない
            int thisIndexX = threeGoSituation.Positions[0].x;
            int thisIndexZ = threeGoSituation.Positions[0].z;
            int tmpIndexY = CheckCanPut(thisIndexX, thisIndexZ);
            if(tmpIndexY != BoardStatus.CanNotPut){
                threeGoSituation[0] = (x: thisIndexX, z:thisIndexZ, y: tmpIndexY);
                if(tmpIndexY == 2){
                    //2連チェックの場合のみ2つめの空き座標を格納する
                    threeGoSituation[1] = (x: thisIndexX, z:thisIndexZ, y: tmpIndexY+1);
                }
            }
        }
        else if(threeGoSituation.Pattern == LinePattern.Pattern4){
            //3連の碁の座標が持っていないZとY座標を抽出、碁が置けるかどうか確認する
            int thisIndexX = threeGoSituation.Positions[0].x;
            int[] thisIndexZ = threeGoSituation.Positions.Select(item => item.z).ToArray();
            int[] notHaveIndexZ = lineIndex.Where(item => !thisIndexZ.Contains(item)).Select(item => item).ToArray();
            int[] notHaveIndexY = notHaveIndexZ.Select(item => threeGoSituation.Positions[0].z == threeGoSituation.Positions[0].y ? item: 3 - item).ToArray();
            for(int index = 0; index < notHaveIndexZ.Length; index++){
                int canPutIndexY = CheckCanPut(thisIndexX, notHaveIndexZ[index]);
                if(canPutIndexY != BoardStatus.CanNotPut | canPutIndexY <= notHaveIndexY[index]){
                    threeGoSituation[index] = (x: thisIndexX, z: notHaveIndexZ[index], y: notHaveIndexY[index]);
                }
            }
        }
        else if(threeGoSituation.Pattern == LinePattern.Pattern5){
            //3連の碁の座標が持っていないXとY座標を抽出、碁が置けるかどうか確認する
            int thisIndexZ = threeGoSituation.Positions[0].z;
            int[] thisIndexX = threeGoSituation.Positions.Select(item => item.x).ToArray();
            int[] notHaveIndexX = lineIndex.Where(item => !thisIndexX.Contains(item)).Select(item => item).ToArray();
            int[] notHaveIndexY = notHaveIndexX.Select(item => threeGoSituation.Positions[0].x == threeGoSituation.Positions[0].y ? item: 3 - item).ToArray();
            for(int index = 0; index < notHaveIndexX.Length; index++){
                int canPutIndexY = CheckCanPut(notHaveIndexX[index], thisIndexZ);
                if(canPutIndexY != BoardStatus.CanNotPut | canPutIndexY <= notHaveIndexY[index]){
                    threeGoSituation[index] = (x: notHaveIndexX[index], z: thisIndexZ, y: notHaveIndexY[index]);
                }
            }
        }
        else if(threeGoSituation.Pattern == LinePattern.Pattern6){
            //3連の碁の座標が持っていないXとZ座標を抽出、碁が置けるかどうか確認する
            int thisIndexY = threeGoSituation.Positions[0].y;
            int[] thisIndexX = threeGoSituation.Positions.Select(item => item.x).ToArray();
            int[] notHaveIndexX = lineIndex.Where(item => !thisIndexX.Contains(item)).Select(item => item).ToArray();
            int[] notHaveIndexZ = notHaveIndexX.Select(item => threeGoSituation.Positions[0].x == threeGoSituation.Positions[0].z ? item: 3 - item).ToArray();
            for(int index = 0; index < notHaveIndexX.Length; index++){
                int canPutIndexY = CheckCanPut(notHaveIndexX[index], notHaveIndexZ[index]);
                if(canPutIndexY != BoardStatus.CanNotPut | canPutIndexY <= thisIndexY){
                    threeGoSituation[0] = (x: notHaveIndexX[index], z: notHaveIndexZ[index], y: thisIndexY);
                }

            }
        }
        else if(threeGoSituation.Pattern == LinePattern.Pattern7){
            //3連の碁の座標が持っていないX、Y、Z座標を抽出、碁が置けるかどうか確認する
            int[] thisIndexX = threeGoSituation.Positions.Select(item => item.x).ToArray();
            int[] notHaveIndexX = lineIndex.Where(item => !thisIndexX.Contains(item)).Select(item => item).ToArray();
            int[] notHaveIndexY;
            int[] notHaveIndexZ;
            //条件式が長く、分岐も多いためif文を使う
            if(threeGoSituation.Positions[0].x == threeGoSituation.Positions[0].z & threeGoSituation.Positions[0].x == threeGoSituation.Positions[0].y){
                notHaveIndexY = notHaveIndexX.Select(item => item).ToArray();
                notHaveIndexZ = notHaveIndexX.Select(item => item).ToArray();
            }
            else if(threeGoSituation.Positions[0].x == threeGoSituation.Positions[0].z & threeGoSituation.Positions[0].x != threeGoSituation.Positions[0].y){
                notHaveIndexY = notHaveIndexX.Select(item => 3 - item).ToArray();
                notHaveIndexZ = notHaveIndexX.Select(item => item).ToArray();
            }
            else if(threeGoSituation.Positions[0].x == threeGoSituation.Positions[0].y & threeGoSituation.Positions[0].x != threeGoSituation.Positions[0].z){
                notHaveIndexY = notHaveIndexX.Select(item => item).ToArray();
                notHaveIndexZ = notHaveIndexX.Select(item => 3 - item).ToArray();
            }
            else{
                notHaveIndexY = notHaveIndexX.Select(item => 3 - item).ToArray();
                notHaveIndexZ = notHaveIndexX.Select(item => 3 - item).ToArray();
            }
            for(int index = 0; index < notHaveIndexX.Length; index++){
                int canPutIndexY = CheckCanPut(notHaveIndexX[index], notHaveIndexZ[index]);
                if(canPutIndexY != BoardStatus.CanNotPut | canPutIndexY <= notHaveIndexY[index]){
                    threeGoSituation[index] = (x: notHaveIndexX[index], z: notHaveIndexZ[index], y: notHaveIndexY[index]);
                }
            }
        }
    }
}