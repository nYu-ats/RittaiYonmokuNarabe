using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using Cysharp.Threading.Tasks;

public class GameResultPanel : MonoBehaviour
{
    [SerializeField] Text winnerText;
    [SerializeField] Text moveCountText;
    [SerializeField] GameController gameController;
    [SerializeField] FirebaseUpdateRecordFunc firebaseUpdateRecord;
    [SerializeField] Button ReturnToHomeButton;
    [SerializeField] Text connectingText;
    [SerializeField] Image drawImage;
    [SerializeField] string whoWin = "勝者 : ";
    [SerializeField] string moveCount =  "手";
    async void Start()
    {
        int winner = gameController.CurrentTurn;
        string playerName = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
        moveCount = (GameRule.TotalGoNumber - gameController.GoNumber).ToString() + moveCount;

        if(winner == gameController.Player){
            whoWin += playerName;
        }
        else if(winner == gameController.Rival){
            whoWin += gameController.RivalName;
        }
        else{
            //ドローだった場合
            whoWin = "ドロー";
            moveCount = "";
        }

        //マルチプレイ時はFirebase上のゲーム結果を更新する
        if(gameController.PlayMode == GameRule.MultiPlayMode & winner != GameRule.DrawSignal){
            await UpdateGameResult(winner, playerName);
        }

        winnerText.text = whoWin;
        moveCountText.text = moveCount;
        ReturnToHomeButton.enabled = true; //全てのリザルト処理が終わった段階でホームへ戻れるようにする
    }

    private async UniTask UpdateGameResult(int winner, string playerName){
        GameRecord currentRecord = null;
        connectingText.enabled = true;
        try{
        await UniTask.Run(async () => {
            currentRecord = await firebaseUpdateRecord.GetRecord(playerName);
        }).ContinueWith(async () => {
            if(winner == gameController.Player){
                await firebaseUpdateRecord.SetRecord(playerName, currentRecord.win + 1, currentRecord.lose);
            }
            else if(winner == gameController.Rival){
                await firebaseUpdateRecord.SetRecord(playerName, currentRecord.win, currentRecord.lose + 1);
            }
        });

        }
        catch{}
        finally{
            connectingText.enabled = false;
        }
    }
}
