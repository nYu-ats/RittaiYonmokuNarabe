using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonConfig;

public class MultiPlayButton : BasePlayButton
{
    [SerializeField] FirebaseMatchingFunc firebaseMatching;
    [SerializeField] Text connectingText;
    [SerializeField] PlaySE playSE;
    public async void OnButtonClicked(int modeNumber){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        loadPlayMode = modeNumber;
        connectingText.enabled = true;
        //マッチング完了を待機
        try{
            (int roomNumber, int matchingPattern) matchingInfo = await firebaseMatching.Matching();
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
        catch{
            connectingText.enabled = false;
        }
    }
}
