using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using Cysharp.Threading.Tasks;

public class GameResultPanel : MonoBehaviour
{
    [SerializeField] Text winnerText;
    [SerializeField] Text moveCountText;
    [SerializeField] GameController gameController;
    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] Button ReturnToHomeButton;
    [SerializeField] Text connectingText;
    [SerializeField] string whoWin = "勝者 : ";
    [SerializeField] string moveCount =  "手";
    async void Start()
    {
        int winner = gameController.CurrentTurn;
        string playerName = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
        if(winner == gameController.Player){
            whoWin += playerName;
        }
        else{
            whoWin += gameController.RivalName;
        }
        moveCount = (GameRule.TotalGoNumber - gameController.GoNumber).ToString() + moveCount;

        //マルチプレイ時はFirebase上のゲーム結果を更新する
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            await UpdateGameResult(winner, playerName);
        }

        winnerText.text = whoWin;
        moveCountText.text = moveCount;
        ReturnToHomeButton.enabled = true; //全てのリザルト処理が終わった段階でホームへ戻れるようにする
    }

    private async UniTask UpdateGameResult(int winner, string playerName){
        GameRecord currentRecord = null;
        connectingText.enabled = true;
        await UniTask.Run(async () => {
            currentRecord = await connectFirebase.GetRecord(playerName);
        }).ContinueWith(async () => {
            if(winner == gameController.Player){
                await connectFirebase.SetRecord(playerName, currentRecord.win + 1, currentRecord.lose);
            }
            else{
                await connectFirebase.SetRecord(playerName, currentRecord.win, currentRecord.lose + 1);
            }
        });
        connectingText.enabled = false;
    }
}
