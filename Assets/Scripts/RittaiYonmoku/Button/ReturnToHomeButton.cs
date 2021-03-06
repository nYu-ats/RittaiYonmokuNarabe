﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class ReturnToHomeButton : MonoBehaviour
{
    [SerializeField] FirebaseCloseGameFunc firebaseCloseGame;
    [SerializeField] GameController gameController;
    [SerializeField] PlaySE playSE;
    [SerializeField] PlayBGM playBGM;
    [SerializeField] DisplayAdvertise displayAdvertise;
    public async void OnClicked(){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        //マルチプレイの時はホームに戻る前にFirebase上のゲームルームを削除する
        try{
            if(gameController.PlayMode == GameRule.MultiPlayMode){
                await UniTask.Run(async () => {
                    await firebaseCloseGame.DeleteGameRoom();
                });
            }
        }
        catch{}
        finally{
            Time.timeScale = 1; //ホーム画面に戻る前にタイムスケールを元に戻す
            playBGM.IsFadeOut = true;
            await UniTask.WaitWhile(() => playBGM.IsFadeOut);
            displayAdvertise.ShowAdvertise();
            await UniTask.WaitUntil(() => displayAdvertise.AdClose);
            SceneManager.LoadScene(GameSceneName.HomeScene);
        }
    }
}
