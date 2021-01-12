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
        int[] winLoseCount = new int[2]{0, 0};
        string userName = PlayerPrefs.GetString("UserName"); //UserNameベタ打ちの部分は要修正
        userNameText.text = userName;
        await UniTask.Run(async () => {
            winLoseCount = await connectFirebase.GetRecord(userName);
        }).ContinueWith(() => {
            winText.text = winLoseCount[0].ToString();
            loseText.text = winLoseCount[1].ToString();
        });
    }

    //
    private async UniTask SetUserWinLoseAsync(string userName, Text win, Text lose){
        int[] winLoseCount = new int[2]{0, 0};
        await UniTask.Run(async () => {
            winLoseCount = await connectFirebase.GetRecord(userName);
        });
        win.text = winLoseCount[0].ToString();
        lose.text = winLoseCount[1].ToString();
    }
}
