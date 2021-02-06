using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonConfig;

public class MultiPlayButton : BasePlayButton
{
    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] Text connectingText;
    public async void OnButtonClicked(int modeNumber){
        loadPlayMode = modeNumber;
        connectingText.enabled = true;
        //マッチング完了を待機
        (int roomNumber, int matchingPattern) matchingInfo = await connectFirebase.Matching();
        gameRoom = matchingInfo.roomNumber;
        if(matchingInfo.matchingPattern == MatchingPattern.CreateRoom){
            playerColor = GameRule.FirstAttack;
            rivalColor = GameRule.SecondAttack;
        }
        else{
            playerColor = GameRule.SecondAttack;
            rivalColor = GameRule.FirstAttack;  
        }
        connectingText.enabled = false; 

        SceneManager.sceneLoaded += SetGameVariable;
        SceneManager.LoadScene(GameSceneName.GameScene);
    }
}
