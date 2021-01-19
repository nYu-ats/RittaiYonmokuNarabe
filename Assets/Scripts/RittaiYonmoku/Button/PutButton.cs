using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class PutButton : MonoBehaviour
{
    [SerializeField] Board board;
    //テスト用
    [SerializeField] int indexX = 0;
    [SerializeField] int indexY = 0;
    private int goColor = BoardStatus.GoWhite;

    public void OnClicked(){
        board.AddGo(indexX, indexY, goColor);
        if(goColor == BoardStatus.GoWhite){
            goColor = BoardStatus.GoBlack;
        }
        else if(goColor == BoardStatus.GoBlack){
            goColor = BoardStatus.GoWhite;
        }
    }
}
