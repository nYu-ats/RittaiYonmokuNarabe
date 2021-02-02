using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommonConfig;

public class ReturnToHomeButton : MonoBehaviour
{
    public void OnClicked(){
        SceneManager.LoadScene(GameSceneName.HomeScene);
    }
}
