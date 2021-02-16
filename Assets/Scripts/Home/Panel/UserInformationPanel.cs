using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class UserInformationPanel : MonoBehaviour
{
    [SerializeField] Text winText;
    [SerializeField] Text loseText;
    [SerializeField] Text userNameText;
    [SerializeField] GameObject connectFailed;
    [SerializeField] FirebaseUpdateRecordFunc firebaseUpdateRecord;
    private string attachToUserNameText = "さんの戦績";
    async void Start()
    {
        string userName = SetUserName();
        await SetWinLoseCount(userName);
    }

    private string SetUserName(){
        string userName = "";
        if(PlayerPrefs.HasKey(PlayerPrefsKey.UserNameKey)){
            //通信エラーでユーザー名が取得できない場合があるので
            //ユーザー名有無の確認
            userName = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
        }
        userNameText.text = userName + attachToUserNameText;
        return userName;
    }

    private async UniTask SetWinLoseCount(string userName){
        //ユーザー名をキーにしてFirebaseから勝敗の履歴を取得する
        try{
            await UniTask.Run(async () => {
                GameRecord winLose = await firebaseUpdateRecord.GetRecord(userName);
                return winLose;
            }).ContinueWith(winLose => {
                winText.text = winLose.win.ToString();
                loseText.text = winLose.lose.ToString();
            });
        }
        catch{
            connectFailed.SetActive(true);
        }
    }
}
