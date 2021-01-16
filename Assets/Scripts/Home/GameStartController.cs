using UnityEngine;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class GameStartController : MonoBehaviour
{
    //PanelオブジェクトだとSetActiveが使えないためGameObjectとしてパネルを入れる
    [SerializeField] GameObject gameTitlePanel;
    [SerializeField] GameObject userRegisterPanel;
    [SerializeField] GameObject homePanel;
    [SerializeField] int titleDisplayTime = 5000;
    
    //ゲーム起動時最初に実行させたいのでAwakeを使う
    async void Awake(){
        //PlayerPrefs.DeleteKey("UserName"); //テスト用
        await DisplayTitlePanel(titleDisplayTime);
        await ChkUserRegister();
        DisplayHomePanel();
    }
    private async UniTask DisplayTitlePanel(int time){
        gameTitlePanel.SetActive(true);
        await UniTask.Delay(time);
        gameTitlePanel.SetActive(false);
    }

    private async UniTask ChkUserRegister(){
        //すでにユーザー名登録がされていた場合は登録パネルの表示はしない
        userRegisterPanel.SetActive(!PlayerPrefs.HasKey(PlayerPrefsKey.UserNameKey));
        await UniTask.WaitUntil(() => PlayerPrefs.HasKey(PlayerPrefsKey.UserNameKey));
    }

    private void DisplayHomePanel(){
        //初回起動時に音量1/2で設定する
        if(!PlayerPrefs.HasKey(PlayerPrefsKey.VolumeKey)){
            PlayerPrefs.SetInt(PlayerPrefsKey.VolumeKey, 1);
        }
        userRegisterPanel.SetActive(false);
        homePanel.SetActive(true);
    }
}

