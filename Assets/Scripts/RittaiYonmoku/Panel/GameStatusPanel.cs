using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class GameStatusPanel : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] GameObject resultUI;
    [SerializeField] GameObject gameResultPanel;
    [SerializeField] int statusDisplayTime = 3000;
    [SerializeField] SyncBoardStatus syncBoardStatus;
    [SerializeField] PlaySE playSE;
    void Start()
    {
        gameController.checkMateEvent += DisplayGameStatus;
        gameController.drawEvent += DisplayGameStatus;
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            syncBoardStatus.rivalGiveUp += DisplayGameStatus;
        }
    }

    private async void DisplayGameStatus(string text){
        playSE.PlaySound(AudioConfig.GameEndIndex);
        resultUI.SetActive(true);
        resultUI.transform.GetChild(0).gameObject.GetComponent<Text>().text = text;
        await UniTask.Delay(statusDisplayTime)
        .ContinueWith(() => gameResultPanel.SetActive(true)); //ゲームリザルトを表示するまで数秒間をあける
        gameController.checkMateEvent -= DisplayGameStatus;
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            syncBoardStatus.rivalGiveUp -= DisplayGameStatus;
        }
    }
}
