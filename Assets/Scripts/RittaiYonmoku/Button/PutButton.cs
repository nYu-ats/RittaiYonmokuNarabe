using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class PutButton : MonoBehaviour
{
    [SerializeField] Board board;
    //テスト用
    private int indexX = 0;
    private int indexY = 0;
    private int goColor = BoardStatus.GoWhite;

    public void OnClicked(){
        board.AddGo(indexX, indexY, goColor);
        indexX += 1;
        indexY += 1;
        if(goColor == BoardStatus.GoWhite){
            goColor = BoardStatus.GoBlack;
        }
        else if(goColor == BoardStatus.GoBlack){
            goColor = BoardStatus.GoWhite;
        }
    }
}
