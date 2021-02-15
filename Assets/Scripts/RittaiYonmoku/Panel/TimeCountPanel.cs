using System;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class TimeCountPanel : MonoBehaviour
{
    public bool DoTimeCount{set {doTimeCount = value;}}
    public delegate void TimeOutEventHandler();
    public event TimeOutEventHandler timeOut = () => {};

    [SerializeField] BoardController boardController;
    [SerializeField] Text timeCountText;
    [SerializeField] GameController gameController;
    private float time;
    private float timeLimit = 90.0f;
    private bool doTimeCount = false; //GameControlerでtrueにセットしてカウントを開始

    void Start(){
        boardController.boardUpdated += ResetTimeCount;
        time = timeLimit;
    }

    void Update()
    {
        if(doTimeCount){
            time -= Time.deltaTime;
            timeCountText.text = String.Format("Time : {0:00}", Mathf.Floor(time));
            if(time <= 0){
                if(gameController.PlayMode == GameRule.SoloPlayMode){
                    //ソロプレイの場合は自身とNPC両ターンでタイムアウトを発生させる
                    timeOut();
                }
                else if(gameController.CurrentTurn == gameController.Player){
                    //マルチプレイの場合は自身のターンのみタイムアウトを発生させる
                    timeOut();
                }
            }
        }
    }

    private void ResetTimeCount(){
        //ターンが切り替わったら制限時間を再セット
        time = timeLimit;
    }

    public void SwitchTimeCountStatus(bool status){
        //時間カウントを止めるかリスタートさせるか
        //シーンがロードされた直後からカウントがスタートされないようにする
        doTimeCount = status;
    }
}
