using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using CommonConfig;

public class Board : MonoBehaviour
{
    //Linqで扱いやすくするためボードの状態は1次元の配列で持つ
    //配列の各要素が度の座標に対応するかは別の配列で管理する
    public (int x, int z, int y)[] posArray = new (int x, int z, int y)[]{};
    public int[] boardStatusArray = Enumerable.Repeat(0, GameRule.TotalGoNumber).ToArray();

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

}

