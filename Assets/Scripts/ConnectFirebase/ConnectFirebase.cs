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
    void SetInfo(string setValue);
}

interface IReadUserName{
    Task<bool> ReadUserName(string userName);
}

interface IReadWinLose{
    int[] ReadWinLose(string userName);
}

interface IMatching{
    int Matching(string userName);
}


public class ConnectFirebase:MonoBehaviour, ISet, IReadUserName, IReadWinLose
{
    public bool connectingFlag = true;
    DatabaseReference reference;

    void Start(){
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public async Task<bool> ReadUserName(string userName){
        return await reference.Child("user").Child(userName).GetValueAsync().ContinueWith(task => {
            Debug.Log(task.Result.GetRawJsonValue()); //テスト用
            if(task.Result.GetRawJsonValue() == null){
                connectingFlag = false;
                return true;
            }
            else{
                connectingFlag = false;
                return false;
            }
        });
    }

    public void SetInfo(string setValue){
        Debug.Log("set");;
    }

    public int[] ReadWinLose(string userName){
        return new int[2]{100, 100};
    }

    public async UniTask<int> Matching(){
        await UniTask.Delay(5000);
        int gameRoomNumber = 100;
        connectingFlag = false;
        return gameRoomNumber;
    }
}

interface IDelete{

}


interface ISubscribe{
    
}

