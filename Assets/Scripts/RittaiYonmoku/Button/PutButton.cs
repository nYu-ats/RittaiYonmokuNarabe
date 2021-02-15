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
    [SerializeField] PlaySE playSE;
    private int goColor;

    void Start(){
        //ボードに置く碁の色の初期設定
        if(gameController.Player == GameRule.FirstAttack){
            goColor = BoardStatus.GoWhite;
        }
        else{
            goColor = BoardStatus.GoBlack;
        }
    }

    public void OnClicked(){
        playSE.PlaySound(AudioConfig.GoPutButtonIndex);
        boardController.AddGo(putPositionPanel.IndexXZ.x, putPositionPanel.IndexXZ.z, goColor);
        //連続してクリックされるのを防ぐため初回クリック時にボタンをfalseにする
        //自分のターンが回ってきたときにはGameControllerからtrueにされる
        this.GetComponent<Button>().enabled = false;
    }
}
