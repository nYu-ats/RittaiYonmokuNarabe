using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

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
            validFlag = await connectFirebase.ReadUserName(inputUserName);
            Debug.Log(validFlag);
            return validFlag;
        }).ContinueWith(async flag => {
        if(flag){
            connectingText.enabled = connectFirebase.waitFlag; //続くユーザー名をDBに格納する処理のため再度"connecting"を表示
            await RegisterUserNameAsync(inputUserName);
            PlayerPrefs.SetString("UserName", inputUserName); //DBへの格納が成功した後にローカルへユーザー名登録
        }
        else{
            ShowAlert(true);
        }});
        //validFlag = await connectFirebase.ReadUserName(inputUserName);
        //await UniTask.WaitWhile(() => connectFirebase.waitFlag); //ユーザー情報読み取り中の間待機する
        
        /*if(validFlag){
            connectingText.enabled = connectFirebase.waitFlag; //続くユーザー名をDBに格納する処理のため再度"connecting"を表示
            await RegisterUserNameAsync(inputUserName);
            await UniTask.WaitWhile(() => connectingText.enabled);
            PlayerPrefs.SetString("UserName", inputUserName); //DBへの格納が成功した後にローカルへユーザー名登録
        }
        else{
            ShowAlert(true);
        }*/
    }

    /*
    private async UniTask<bool> CheckUserNameAsync(string name){
        bool tRead = await connectFirebase.ReadUserName(name);
        connectingText.enabled = false; //awaitしている処理が完了次第"connecting"を非表示化
        return tRead;
    }
    */

    private async UniTask RegisterUserNameAsync(string name){
        await UniTask.Delay(3000);
        await UniTask.Run(() => connectFirebase.SetInfo(name));
        connectingText.enabled = false;
    }
}

