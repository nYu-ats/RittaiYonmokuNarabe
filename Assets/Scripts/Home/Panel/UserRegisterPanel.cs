using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class UserRegisterPanel : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text inputText;
    [SerializeField] Text userNameAlreadyExist;

    private string inputUserName = "";

    //フォームへの入力内容をテキストフィールドに反映させる
    public void ReflectInputText(){
        inputText.text = inputField.text;
        inputUserName = inputField.text;
    }

    //すでに使用済みの名前を入力していた場合はアラートを出す
    public void ShowAlert(bool status){
        userNameAlreadyExist.enabled = status;
    }


    /*
    下記ボタン処理
    UserRegisterからの分離の検討が必要
    */
    [SerializeField] Text connectingText;

    [SerializeField] ConnectFirebase connectFirebase;
    private bool validFlag = false;



    public async void ButtonClicked(){
        connectingText.enabled = true; //通信処理に入る直前に通信中メッセージを表示する
        await UniTask.Run(async() => {
            validFlag = await connectFirebase.CheckUserNameValid(inputUserName);
            return validFlag;
        }).ContinueWith(async flag => {
        if(flag){
            await connectFirebase.SetUserName(inputUserName);
            PlayerPrefs.SetString(PlayerPrefsKey.UserNameKey, inputUserName); //DBへの格納が成功した後にローカルへユーザー名登録
            await connectFirebase.SetRecord(PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey), 50, 50);
            connectingText.enabled = false;
        }
        else{
            connectingText.enabled = false;
            ShowAlert(true);
        }});
    }
}

