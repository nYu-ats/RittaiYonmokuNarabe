using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class ReturnToHomeButton : MonoBehaviour
{
    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] GameController gameController;
    public async void OnClicked(){
        await UniTask.Run(async () => {
            await connectFirebase.DeleteGameRoom();
        });
        Time.timeScale = 1; //タイムスケールを元に戻す
        this.GetComponent<Button>().enabled = false; //ホームに戻る前にdisableに戻す
        SceneManager.LoadScene(GameSceneName.HomeScene);
    }
}
