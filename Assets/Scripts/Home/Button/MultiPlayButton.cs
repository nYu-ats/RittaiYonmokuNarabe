using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonConfig;
using Cysharp.Threading.Tasks;

public class MultiPlayButton : BasePlayButton
{
    [SerializeField] FirebaseMatchingFunc firebaseMatching;
    [SerializeField] Text connectingText;
    [SerializeField] PlaySE playSE;
    [SerializeField] PlayBGM playBGM;
    [SerializeField] GameObject connectFailedPanel;
    public async void OnButtonClicked(int modeNumber){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        this.GetComponent<Button>().enabled = false; //複数回タップされるのを防ぐため
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
            playBGM.IsFadeOut = true;
            await UniTask.WaitWhile(() => playBGM.IsFadeOut);
            SceneManager.sceneLoaded += SetGameVariable;
            SceneManager.LoadScene(GameSceneName.GameScene);
        }
        catch{
            connectingText.enabled = false;
            connectFailedPanel.SetActive(true);
            this.GetComponent<Button>().enabled = true;
        }
    }
}
