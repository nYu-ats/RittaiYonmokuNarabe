using UnityEngine;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class RegisterButton : MonoBehaviour
{
    [SerializeField] ConnectFirebase connectFirebase;
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
        connectingEvent(true); //通信処理に入る直前に通信中メッセージを表示する
        fetchUserNameEvent();
        await UniTask.Run(async() => {
            validFlag = await connectFirebase.UserNameValidation(inputUserName);
            return validFlag;
        }).ContinueWith(async flag => {
        if(flag){
            await connectFirebase.SetUserName(inputUserName);
            PlayerPrefs.SetString(PlayerPrefsKey.UserNameKey, inputUserName); //DBへの格納が成功した後にローカルへユーザー名登録
            await connectFirebase.SetRecord(PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey), 0, 0); //初回なので勝敗共に0
            connectingEvent(false);
        }
        else{
            connectingEvent(false);
            showAlertEvent(true);
        }});
    }
}
