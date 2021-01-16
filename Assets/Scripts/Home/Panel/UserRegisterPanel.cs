using UnityEngine;
using UnityEngine.UI;

public class UserRegisterPanel : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text inputText;
    [SerializeField] Text userNameAlreadyExist;
    [SerializeField] Text connectingText;
    [SerializeField] Button registerButton;

    private string inputUserName = "";

    void Start(){
        //スタート時に登録ボタンの各イベントにイベントハンドラーを登録する
        registerButton.GetComponent<RegisterButton>().showAlertEvent += ShowAlert;
        registerButton.GetComponent<RegisterButton>().connectingEvent += ShowConnecting;
        registerButton.GetComponent<RegisterButton>().fetchUserNameEvent += SetSendUserName;
    }

    //フォームへの入力内容をテキストフィールドに反映させるメソッド
    public void ReflectInputText(){
        inputText.text = inputField.text;
        inputUserName = inputField.text;
    }

    //すでに使用済みの名前を入力していた場合はアラートを出す
    private void ShowAlert(bool status){
        userNameAlreadyExist.enabled = status;
    }

    //送信ボタンがクリックされたときに入力済みのユーザー名をセットする
    public void SetSendUserName(){
        registerButton.GetComponent<RegisterButton>().inputUserName = inputField.text;
    }

    //通信中にconnectingテキストを表示、終わり次第非表示にする
    public void ShowConnecting(bool status){
        connectingText.enabled = status;
    }
}

