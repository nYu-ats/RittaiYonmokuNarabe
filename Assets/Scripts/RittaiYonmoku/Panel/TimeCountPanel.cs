using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountPanel : MonoBehaviour
{
    [SerializeField] BoardController boardController;
    [SerializeField] Text timeCountText;
    [SerializeField] GameController gameController;
    private float time;
    private float timeLimit = 90.0f;
    private bool doTimeCount = true;

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
                //時間切れになったらランダムで碁を置く
                RandomPut();
            }
        }
    }

    private void ResetTimeCount(){
        //ターンが切り替わったら制限時間を再セット
        time = timeLimit;
    }

    public void SwitchTimeCountStatus(bool status){
        //時間カウントを止めるかリスタートさせるか
        doTimeCount = status;
    }

    private void RandomPut(){
        //制限時間を過ぎた場合には適当な場所に碁を置く
        GoSituations[] canPutPos = boardController.VacantPos();
        if(canPutPos != null){
            int rndIndex = UnityEngine.Random.Range(0, canPutPos.Length);
            boardController.AddGo(canPutPos[rndIndex].Positions[0].x, canPutPos[rndIndex].Positions[0].z, gameController.CurrentTurn);
        }
    }
}
