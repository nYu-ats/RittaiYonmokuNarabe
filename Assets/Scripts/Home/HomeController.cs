using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    [SerializeField] GameObject userRegisterPanel;  //PanelオブジェクトだとSetActiveが使えないためGameObjectとしてパネルを入れる
    void Awake()
    {
        userRegisterPanel.SetActive(CheckUserRegisterDone());
    }
    private bool CheckUserRegisterDone()
    {
        //すでにユーザー名登録がされていた場合は登録パネルの表示はしない
        if(PlayerPrefs.HasKey("UserName"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
