using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;


public class GameStartController : MonoBehaviour
{
    //PanelオブジェクトだとSetActiveが使えないためGameObjectとしてパネルを入れる
    [SerializeField] GameObject gameTitlePanel;
    [SerializeField] GameObject userRegisterPanel;
    [SerializeField] GameObject HomePanel;
    private string userNameKey = "UserName"; //他でも使う共通設定はどこかにまとめた方がよい?
    private int titleDisplayTime = 5000;
    
    //ゲーム起動時最初に実行させたいのでAwakeを使う
    async void Awake(){
        PlayerPrefs.DeleteKey("UserName"); //テストのため、最初にusernameのキーを消去
        gameTitlePanel.SetActive(true);
        await UniTask.Delay(titleDisplayTime); //タイトルパネルを1秒間表示
        gameTitlePanel.SetActive(false);
        //すでにユーザー名登録がされていた場合は登録パネルの表示はしない
        userRegisterPanel.SetActive(!PlayerPrefs.HasKey(userNameKey));
        await UniTask.WaitUntil(() => PlayerPrefs.HasKey(userNameKey)); //ユーザー登録が完了するまで待機
        Debug.Log(PlayerPrefs.GetString("UserName")); //ユーザー登録テスト用
        CheckPlayerSettingExists(); 
        userRegisterPanel.SetActive(false);
        HomePanel.SetActive(true);
    }

    private void CheckPlayerSettingExists(){
        if(!PlayerPrefs.HasKey("Volume")){
            PlayerPrefs.SetInt("Volume", 1);
        }
    }
}

