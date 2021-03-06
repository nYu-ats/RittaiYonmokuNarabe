﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonConfig;
using Cysharp.Threading.Tasks;

public class SoloPlayButton : BasePlayButton
{
    [SerializeField] PlaySE playSE;
    [SerializeField] PlayBGM playBGM;
    public async void OnButtonClicked(int modeNumber){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        this.GetComponent<Button>().enabled = false;
        await UniTask.Delay(500); //ボタンクリック音の再生を待つ
        loadPlayMode = modeNumber;
        //ソロプレイ時の先行後攻はランダムに決める
        System.Random rnd = new System.Random();
        if(rnd.Next(0, 2) % 2 == 0){
            playerColor = GameRule.FirstAttack;
            rivalColor = GameRule.SecondAttack;
        }
        else{
            playerColor = GameRule.SecondAttack;
            rivalColor = GameRule.FirstAttack;
        }
        playBGM.IsFadeOut = true; //シーン
        await UniTask.WaitWhile(() => playBGM.IsFadeOut);
        SceneManager.sceneLoaded += SetGameVariable;
        SceneManager.LoadScene(GameSceneName.GameScene);
    }
}
