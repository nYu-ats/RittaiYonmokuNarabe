using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterButton : BaseButtonControllerHome
{
    private string sendText = "";

    public string SendText{set{this.sendText = value;}} //ユーザー登録パネルより入力されたユーザー名を受け取るためのプロパティ
    public delegate void UserRegisterEventHandler();
    public event UserRegisterEventHandler SuccessUserRegisterEvent = () => {}; //ユーザー登録が成功した場合のイベント初期化
    public event UserRegisterEventHandler FalseUserRegisterEvent = () => {}; //ユーザー登録が失敗した場合のイベント

    public override void OnClickedAction()
    {
        Debug.Log(sendText);
    }
}