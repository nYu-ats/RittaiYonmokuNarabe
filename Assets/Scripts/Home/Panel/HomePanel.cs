using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

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

    private const string thisAppId = "mitei"; //要修正
    private const string GoogleAppStoreURL = "https://play.google.com/store/apps/details?id=";
    public void ReviewButtonClicked(){
        Application.OpenURL(GoogleAppStoreURL + thisAppId);
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
        SceneManager.LoadScene("RittaiYonmoku");
    }

    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] Text connectingText;

    public async void MultiPlayButtonClicked(int modeNumber){
        loadPlayMode = modeNumber;
        connectingText.enabled = connectFirebase.waitFlag;
        gameRoom = await connectFirebase.Matching();
        await UniTask.WaitWhile(() => connectFirebase.waitFlag);
        connectingText.enabled = connectFirebase.waitFlag; 

        SceneManager.sceneLoaded += SetPlayMode;
        SceneManager.LoadScene("RittaiYonmoku");
    }

    private void SetPlayMode(Scene loadScene, LoadSceneMode mode){
        GameController gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        gameController.playMode = loadPlayMode;
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
