using UnityEngine;
using Cysharp.Threading.Tasks;
using CommonConfig;

public class HomeController : MonoBehaviour
{
    //PanelオブジェクトだとSetActiveが使えないためGameObjectとしてパネルを入れる
    [SerializeField] GameObject userRegisterPanel;
    [SerializeField] GameObject homePanel;
    [SerializeField] GameObject playBGM;
    private static int homeReadCount = 1; //ホーム画面の読み込み回数をカウントする
    public static int HomeReadCount{get {return homeReadCount;}}
    
    //ゲーム起動時最初に実行させたいのでAwakeを使う
    async void Awake(){
        //初回起動時の音量設定
        if(!PlayerPrefs.HasKey(PlayerPrefsKey.VolumeKey)){
            PlayerPrefs.SetInt(PlayerPrefsKey.VolumeKey, 2);
        }
        if(!PlayerPrefs.HasKey(PlayerPrefsKey.BgmVolumeKey)){
            PlayerPrefs.SetInt(PlayerPrefsKey.BgmVolumeKey, 2);
        }

        //ユーザー名登録が完了してからホーム画面を表示する
        await ChkUserRegister();
        DisplayHomePanel();
        playBGM.SetActive(true);
        homeReadCount += 1;
    }

    private async UniTask ChkUserRegister(){
        //すでにユーザー名登録がされていた場合は登録パネルの表示はしない
        userRegisterPanel.SetActive(!PlayerPrefs.HasKey(PlayerPrefsKey.UserNameKey));
        await UniTask.WaitUntil(() => PlayerPrefs.HasKey(PlayerPrefsKey.UserNameKey));
    }

    private void DisplayHomePanel(){
        userRegisterPanel.SetActive(false);
        homePanel.SetActive(true);
    }
}

