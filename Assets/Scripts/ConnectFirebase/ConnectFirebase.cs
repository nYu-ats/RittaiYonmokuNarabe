using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;


interface ISet{
    void SetInfo(string setUserName);
}

interface IReadUserName{
    UniTask<bool> ReadUserName(string userName);
}

interface IReadWinLose{
    int[] ReadWinLose(string userName);
}

interface IMatching{
    int Matching(string userName);
}


public class ConnectFirebase:MonoBehaviour, ISet, IReadUserName, IReadWinLose
{
    public bool waitFlag = true; //呼び出し元を待機させるためのフラグ
    DatabaseReference reference;


    void Start(){
        //Start or Awakeメソッド内でreference作らないといけないらしい
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public async UniTask<bool> ReadUserName(string userName){
        return await reference.Child("user").Child(userName).GetValueAsync().ContinueWith(task => {
            Debug.Log(task.Result.GetRawJsonValue()); //テスト用
            if(task.Result.GetRawJsonValue() == null){
                waitFlag = false;
                return true;
            }
            else{
                waitFlag = false;
                return false;
            }
        });
    }

    public async void SetInfo(string setUserName){
        await reference.Child("user").Child(setUserName).SetValueAsync(true);
    }

    public int[] ReadWinLose(string userName){
        return new int[2]{100, 100};
    }

    public async UniTask<int> Matching(){
        await UniTask.Delay(5000);
        int gameRoomNumber = 100;
        waitFlag = false;
        return gameRoomNumber;
    }
}

interface IDelete{

}


interface ISubscribe{
    
}

