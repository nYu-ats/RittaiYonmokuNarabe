using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;


interface ISetUserName{
    UniTask SetUserName(string setUserName);
}

interface ICheckUserNameValid{
    UniTask<bool> CheckUserNameValid(string userName);
}

interface IMatching{
    int Matching(string userName);
}

interface ISetRecord{
    UniTask SetRecord(string userName, int winCount, int loseCount);
}

interface IGetRecord{
    UniTask<GameRecord> GetRecord(string userName);
}

public class GameRecord
{
    public int win;
    public int lose;

    public GameRecord(int winCount, int loseCount){
        this.win = winCount;
        this.lose = loseCount;
    }
}


public class ConnectFirebase:MonoBehaviour, ISetUserName, ICheckUserNameValid, ISetRecord, IGetRecord
{
    public bool waitFlag = true; //呼び出し元を待機させるためのフラグ
    DatabaseReference reference;


    void Start(){
        //Start or Awakeメソッド内でreference作らないといけないらしい
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public async UniTask<bool> CheckUserNameValid(string userName){
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

    public async UniTask SetUserName(string setUserName){
        await reference.Child("user").Child(setUserName).SetValueAsync(true);
    }

    public async UniTask SetRecord(string userName, int winCount, int loseCount){
        GameRecord gameRecord = new GameRecord(winCount, loseCount);
        string json = JsonUtility.ToJson(gameRecord);
        await reference.Child("record").Child(userName).SetRawJsonValueAsync(json);
    }

    //レコードがなかった場合の処理が必要?
    public async UniTask<GameRecord> GetRecord(string userName){
        return await reference.Child("record").Child(userName).GetValueAsync().ContinueWith(task => {
            if(task.Result.GetRawJsonValue() != null){
                GameRecord gameRecord = JsonUtility.FromJson<GameRecord>(task.Result.GetRawJsonValue());
                return gameRecord;
            }
            else{
                return new GameRecord(0, 0);
            }
        });
    }

    public async UniTask<int> Matching(){
        await UniTask.Delay(5000);
        int gameRoomNumber = 100;
        waitFlag = false;
        return gameRoomNumber;
    }
}