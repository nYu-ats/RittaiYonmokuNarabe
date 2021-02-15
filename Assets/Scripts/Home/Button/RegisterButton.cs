﻿using UnityEngine;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class RegisterButton : MonoBehaviour
{
    [SerializeField] FirebaseUserRgisterFunc firebaseUserRegister;
    [SerializeField] FirebaseUpdateRecordFunc firebaseUpdateRecord;
    [SerializeField] PlaySE playSE;
    public string inputUserName = "";
    private bool validFlag = false;

    //ユーザー名登録が拒否された場合にアラートを出すイベント
    public delegate void UserNameRejectedEventHandler(bool status);
    public event UserNameRejectedEventHandler showAlertEvent = (bool status) => {};

    //通信中のテキストを表示させるためのイベント
    public delegate void ConnectingEventHandler(bool status);
    public event ConnectingEventHandler connectingEvent = (bool status) => {};

    //登録するユーザー名を取得するためのイベント
    public delegate void FetchUserNameEventHandler();
    public event FetchUserNameEventHandler fetchUserNameEvent = () => {};

    public async void ButtonClicked(){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        connectingEvent(true); //通信処理に入る直前に通信中メッセージを表示する
        fetchUserNameEvent();
        await UniTask.Run(async() => {
            validFlag = await firebaseUserRegister.UserNameValidation(inputUserName);
            return validFlag;
        }).ContinueWith(async flag => {
        if(flag){
            await firebaseUserRegister.SetUserName(inputUserName);
            PlayerPrefs.SetString(PlayerPrefsKey.UserNameKey, inputUserName); //DBへの格納が成功した後にローカルへユーザー名登録
            await firebaseUpdateRecord.SetRecord(PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey), 0, 0); //初回なので勝敗共に0
            connectingEvent(false);
        }
        else{
            connectingEvent(false);
            showAlertEvent(true);
        }});
    }
}
