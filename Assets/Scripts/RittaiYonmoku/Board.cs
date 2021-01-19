using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CommonConfig;

interface IAddGo{
    //Y軸について碁が積み上げられていくだけなので、碁を追加する際はX及びZのインデックスのみを指定
    void AddGo(int xIndex, int zIndex, int addColor);
}
public class Board : MonoBehaviour, IAddGo
{
    [SerializeField] GoGenerator goGenerator;
    //4×4×4のボードを表す
    //配列の切り出しが行いやすいためジャグ配列を使う
    //碁が置かれていない状態0で初期化
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

    public void AddGo(int xIndex, int zIndex, int addColor){
        int yIndex = PutYIndex(xIndex, zIndex);
        if(yIndex != BoardStatus.CanNotPut){
            boardArray[xIndex][zIndex][yIndex] = addColor;
            goGenerator.PutGo(xIndex, zIndex, addColor);
        }
        else{
            return;
        }
    }

    private int PutYIndex(int xIndex, int zIndex){
        //碁が追加されるY方向のインデックスを取得する
        try{
            return boardArray[xIndex][zIndex].Select((item, index) => new {Index = index, Value = item})
            .Where(item => item.Value == 0 ).Select(item => item.Index).First();
        }
        catch{
            //すでに4つの碁が置かれている場合
            return BoardStatus.CanNotPut;
        }
    }
}
