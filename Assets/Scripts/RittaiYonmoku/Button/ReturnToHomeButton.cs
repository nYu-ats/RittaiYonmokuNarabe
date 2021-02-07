using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommonConfig;

public class ReturnToHomeButton : MonoBehaviour
{
    public void OnClicked(){
        Time.timeScale = 1; //タイムスケールを元に戻す
        SceneManager.LoadScene(GameSceneName.HomeScene);
    }
}
