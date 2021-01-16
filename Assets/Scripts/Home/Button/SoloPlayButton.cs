using UnityEngine.SceneManagement;
using CommonConfig;

public class SoloPlayButton : BasePlayButton
{
    public void OnButtonClicked(int modeNumber){
        loadPlayMode = modeNumber;
        SceneManager.sceneLoaded += SetPlayMode;
        SceneManager.LoadScene(GameSceneName.GameScene);
    }
}
