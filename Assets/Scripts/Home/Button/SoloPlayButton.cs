using System;
using UnityEngine.SceneManagement;
using CommonConfig;

public class SoloPlayButton : BasePlayButton
{
    public void OnButtonClicked(int modeNumber){
        loadPlayMode = modeNumber;
        //ソロプレイ時の先行後攻はランダムに決める
        Random rnd = new Random();
        if(rnd.Next(0, 2) % 2 == 0){
            playerColor = GameRule.FirstAttack;
            rivalColor = GameRule.SecondAttack;
        }
        else{
            playerColor = GameRule.SecondAttack;
            rivalColor = GameRule.FirstAttack;
        }
        SceneManager.sceneLoaded += SetGameVariable;
        SceneManager.LoadScene(GameSceneName.GameScene);
    }
}
