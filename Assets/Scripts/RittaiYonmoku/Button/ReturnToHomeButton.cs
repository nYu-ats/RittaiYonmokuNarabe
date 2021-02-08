using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonConfig;

public class ReturnToHomeButton : MonoBehaviour
{
    public void OnClicked(){
        Time.timeScale = 1; //タイムスケールを元に戻す
        this.GetComponent<Button>().enabled = false; //ホームに戻る前にdisableに戻す
        SceneManager.LoadScene(GameSceneName.HomeScene);
    }
}
