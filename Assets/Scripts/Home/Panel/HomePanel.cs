﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class HomePanel : MonoBehaviour
{
    [SerializeField] GameObject volumeSettingPanel;
    [SerializeField] GameObject returnButton;
    private GameObject isActivePanel;

    public void VolumeSettingButtonClicked(GameObject isActiveObj){
        volumeSettingPanel.SetActive(true);
        returnButton.SetActive(true);
        isActivePanel = isActiveObj;
    }

    public void ReeturnButtonClicked(){
        isActivePanel.SetActive(false);
        returnButton.SetActive(false);
    }
    
    public void ReviewButtonClicked(){
        Application.OpenURL(URL.GoogleAppStoreURL + URL.ThisAppId);
    }

    int loadPlayMode = 0;
    int gameRoom = 0;
    enum PlayMode
    {
        Solo = 1,
        Multi = 2
    }

    public void SoloPlayButtonClicked(int modeNumber){
        loadPlayMode = modeNumber;
        SceneManager.sceneLoaded += SetPlayMode;
        SceneManager.LoadScene(GameSceneName.GameScene);
    }

    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] Text connectingText;

    public async void MultiPlayButtonClicked(int modeNumber){
        loadPlayMode = modeNumber;
        connectingText.enabled = true;
        gameRoom = await connectFirebase.Matching();
        //await UniTask.WaitUntil(() => connectFirebase.matchingFlag);
        connectingText.enabled = false; 

        SceneManager.sceneLoaded += SetPlayMode;
        SceneManager.LoadScene(GameSceneName.GameScene);
    }

    //ゲーム本体のシーンが読み込まれたときのプレイモードの設定をする
    //加えてゲームシーン->ホームシーンの時には呼び出す必要がないのでイベントハンドラーから消去しておく
    private void SetPlayMode(Scene loadScene, LoadSceneMode mode){
        GameController gameController = GameObject.FindWithTag(Tags.InRittaiYonmoku.GameController).GetComponent<GameController>();
        gameController.playMode = loadPlayMode;
        //プレイモードが増えた時に分岐を追加しやすくするためswitchを使用
        switch(loadPlayMode){
            case (int)PlayMode.Solo:
            goto default;

            case (int)PlayMode.Multi:
            gameController.gameRoom = gameRoom;
            goto default;

            default:
            SceneManager.sceneLoaded -= SetPlayMode;
            break;

        }
    }
}
