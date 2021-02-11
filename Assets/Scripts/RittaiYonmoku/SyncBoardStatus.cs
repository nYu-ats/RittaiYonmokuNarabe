﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using Cysharp.Threading.Tasks;
using CustomException;

public class SyncBoardStatus : MonoBehaviour
{
    [SerializeField] BoardController boardController;
    [SerializeField] GameController gameController;
    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] GameObject gameResultPanel;
    [SerializeField] TimeCountPanel timeCountPanel;
    [SerializeField] Text connectingText;  
    void Start(){
        gameController.setUpComplete += InitializeSyncStatus;
    }

    private void InitializeSyncStatus(){
        if(gameController.Player == GameRule.FirstAttack){
            boardController.boardUpdated += SyncBoard;
        }
        else{
            ListenRival();
        }
        gameController.setUpComplete -= InitializeSyncStatus;
    }

    private void SyncBoard(){
        SetGo();
        ListenRival();
    }

    private void SetGo(){
        connectingText.enabled = true;
        UniTask.Create(async () => {
            await connectFirebase.SetGo(boardController.LastUpdate);
            boardController.boardUpdated -= SyncBoard; //相手のボード更新時に呼び出せれないようにする
            connectingText.enabled = false;
        }).Forget();
    }

    private void ListenRival(){
        connectingText.enabled = true;
        try{
            UniTask.Create(async () => {
                (int x, int z, int y, int color) rivalAction = await connectFirebase.WaitRivalAction();
                boardController.AddGo(rivalAction.x, rivalAction.z, rivalAction.color);
                boardController.boardUpdated += SyncBoard;
            }).Forget();
        }
        catch (GiveUpSignalReceive){
            timeCountPanel.SwitchTimeCountStatus(false);
            gameResultPanel.SetActive(true);
        }
        finally{
            connectingText.enabled = false;
        }
    }

    public void GiveUpAction(){
        connectingText.enabled = true;
        UniTask.Create(async () => {
            boardController.boardUpdated -= SyncBoard;
            await connectFirebase.SetGo((GameRule.GiveRpSignal, GameRule.GiveRpSignal, GameRule.GiveRpSignal, GameRule.GiveRpSignal));
            connectingText.enabled = false;
        }).Forget();
    }
}
