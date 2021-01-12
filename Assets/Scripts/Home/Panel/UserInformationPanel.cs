using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class UserInformationPanel : MonoBehaviour
{
    [SerializeField] Text winText;
    [SerializeField] Text loseText;
    [SerializeField] Text userNameText;
    [SerializeField] ConnectFirebase connectFirebase;
    async void Start()
    {
        string userName = PlayerPrefs.GetString("UserName"); //UserNameベタ打ちの部分は要修正
        userNameText.text = userName;
        await UniTask.Run(async () => {
            GameRecord winLose = await connectFirebase.GetRecord(userName);
            return winLose;
        }).ContinueWith(winLose => {
            winText.text = winLose.win.ToString();
            loseText.text = winLose.lose.ToString();
        });
    }
}
