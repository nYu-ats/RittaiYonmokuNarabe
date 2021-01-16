﻿using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class UserInformationPanel : MonoBehaviour
{
    [SerializeField] Text winText;
    [SerializeField] Text loseText;
    [SerializeField] Text userNameText;
    [SerializeField] ConnectFirebase connectFirebase;
    async void Start()
    {
        string userName = SetUserName();
        await SetWinLoseCount(userName);
    }

    private string SetUserName(){
        string userName = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
        userNameText.text = userName;
        return userName;
    }

    private async UniTask SetWinLoseCount(string userName){
        //ユーザー名をキーにしてFirebaseから勝敗の履歴を取得する
        await UniTask.Run(async () => {
            GameRecord winLose = await connectFirebase.GetRecord(userName);
            return winLose;
        }).ContinueWith(winLose => {
            winText.text = winLose.win.ToString();
            loseText.text = winLose.lose.ToString();
        });
    }
}
