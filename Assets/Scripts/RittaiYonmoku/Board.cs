using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CommonConfig;

interface IAddGo{
    //Y軸についてはユーザー指定ができないため碁を追加する際はX及びZのインデックスで行う
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
        int yIndex = CanPutZIndex(xIndex, zIndex);
        Debug.Log(zIndex);
        if(yIndex != BoardStatus.CanNotPut){
            boardArray[xIndex][zIndex][zIndex] = addColor;
            goGenerator.PutGo(xIndex, zIndex, addColor);
        }
        else{
            return;
        }
    }

    private int CanPutZIndex(int xIndex, int zIndex){
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
