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
        gameRoom = await connectFirebase.Matching();
        connectingText.enabled = false; 

        SceneManager.sceneLoaded += SetPlayMode;
        SceneManager.LoadScene(GameSceneName.GameScene);
    }
}
