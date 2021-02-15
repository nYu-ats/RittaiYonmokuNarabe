using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class ReturnToHomeButton : MonoBehaviour
{
    [SerializeField] FirebaseCloseGameFunc firebaseCloseGame;
    [SerializeField] GameController gameController;
    [SerializeField] PlaySE playSE;
    public async void OnClicked(){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        //マルチプレイの時はホームに戻る前にFirebase上のゲームルームを削除する
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            await UniTask.Run(async () => {
                await firebaseCloseGame.DeleteGameRoom();
            });
        }
        Time.timeScale = 1; //ホーム画面に戻る前にタイムスケールを元に戻す
        SceneManager.LoadScene(GameSceneName.HomeScene);
    }
}
