using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class PutButton : MonoBehaviour
{
    [SerializeField] Board board;
    //テスト用
    [SerializeField] PutPositionPanel putPositionPanel;
    private int goColor = BoardStatus.GoWhite;

    public void OnClicked(){
        board.AddGo(putPositionPanel.IndexXZ.x, putPositionPanel.IndexXZ.z, goColor);
        if(goColor == BoardStatus.GoWhite){
            goColor = BoardStatus.GoBlack;
        }
        else if(goColor == BoardStatus.GoBlack){
            goColor = BoardStatus.GoWhite;
        }
    }
}
