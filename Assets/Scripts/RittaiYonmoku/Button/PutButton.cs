using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class PutButton : MonoBehaviour
{
    [SerializeField] BoardController boardController;
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
        boardController.AddGo(putPositionPanel.IndexXZ.x, putPositionPanel.IndexXZ.z, goColor);
        this.GetComponent<Button>().enabled = false;
    }
}
