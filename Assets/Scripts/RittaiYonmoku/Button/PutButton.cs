using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class PutButton : MonoBehaviour
{
    [SerializeField] Board board;
    //テスト用
    [SerializeField] PutPositionPanel putPositionPanel;
    [SerializeField] GameController gameController;
    private int goColor;

    void Start(){
        if(gameController.Player == GameRule.FirstAttack){
            goColor = BoardStatus.GoWhite;
        }
        else{
            goColor = BoardStatus.GoBlack;
        }
    }

    public void OnClicked(){
        board.AddGo(putPositionPanel.IndexXZ.x, putPositionPanel.IndexXZ.z, goColor);
    }
}
