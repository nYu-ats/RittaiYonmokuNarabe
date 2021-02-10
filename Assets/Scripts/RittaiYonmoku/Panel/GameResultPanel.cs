using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using Cysharp.Threading.Tasks;

public class GameResultPanel : MonoBehaviour
{
    [SerializeField] Text winnerText;
    [SerializeField] Text numberOfMove;
    [SerializeField] GameController gameController;
    [SerializeField] ConnectFirebase connectFirebase;
    [SerializeField] Button ReturnToHomeButton;
    async void Start()
    {
        int winner = gameController.CurrentTurn;
        int restGoNumber = gameController.GoNumber;
        string player = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
        GameRecord currentRecord = null;
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            await UniTask.Run(async () => {
            currentRecord = await connectFirebase.GetRecord(player);
            });
        }

        if(winner == gameController.Player){
            winnerText.text = "勝者 : " + player;
            if(gameController.PlayMode == GameRule.MultiPlayMode){
                    await connectFirebase.SetRecord(player, currentRecord.win + 1, currentRecord.lose);
                }
            }
        else{
            winnerText.text = "勝者 : " + gameController.RivalName;
            if(gameController.PlayMode == GameRule.MultiPlayMode){
                    await connectFirebase.SetRecord(player, currentRecord.win, currentRecord.lose + 1);
                }
        }
        numberOfMove.text = (GameRule.TotalGoNumber - restGoNumber).ToString() + "手";
        ReturnToHomeButton.enabled = true; //全てのリザルト処理が終わった段階でホームへ戻れるようにする
    }
}
