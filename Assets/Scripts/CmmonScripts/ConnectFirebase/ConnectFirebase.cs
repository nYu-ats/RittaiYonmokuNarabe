using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;
using System.Linq;
using FirebaseChildKey;
using CustomException;

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
    DatabaseReference reference;
    void Start(){
        //Start or Awakeメソッド内でreference作らないといけないらしい
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public async UniTask<bool> CheckUserNameValid(string userName){
        return await reference.Child("user").Child(userName).GetValueAsync().ContinueWith(task => {
            Debug.Log(task.Result.GetRawJsonValue()); //テスト用
            if(task.Result.GetRawJsonValue() == null){
                return true;
            }
            else{
                return false;
            }
        });
    }

    public async UniTask SetUserName(string setUserName){
        await reference.Child(GetKey.UserKey).Child(setUserName).SetValueAsync(true);
    }

    public async UniTask SetRecord(string userName, int winCount, int loseCount){
        GameRecord gameRecord = new GameRecord(winCount, loseCount);
        string json = JsonUtility.ToJson(gameRecord);
        await reference.Child(GetKey.RecordKey).Child(userName).SetRawJsonValueAsync(json);
    }

    public async UniTask<GameRecord> GetRecord(string userName){
        try{
            return await reference.Child(GetKey.RecordKey).Child(userName).GetValueAsync().ContinueWith(task => {
            if(task.Result.GetRawJsonValue() != null){
                GameRecord gameRecord = JsonUtility.FromJson<GameRecord>(task.Result.GetRawJsonValue());
                return gameRecord;
            }
            else{
                throw new UserRecordNullException();
            }
        });
        }
        catch (UserRecordNullException){
            //ユーザーレコードがない初回ログイン時はwin : 0/lose : 0でセットされる
            await SetRecord(userName, 0, 0);
            return await GetRecord(userName);
        }
    }

    public bool matchingFlag = false; //マッチング待機するためのフラグ
    public string gameRoomNumber = "";

    public async UniTask<int> Matching(){
        try{
            string gameRoom = "";
            return await UniTask.Run(async () => {
                gameRoom = await CheckMatchingRoom();
            }).ContinueWith(async () => {
                if(gameRoom != null){
                await reference.Child(GetKey.MatchingRoomKey).Child(gameRoom).SetValueAsync(true);
                return int.Parse(gameRoom);
            }
            else{
                gameRoomNumber = MakeMathingRoomNumber(); //ゲームルームの番号を発行する
                //値falseで初期セットしておき、trueになるまで待機する
                await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(false);
                reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).ValueChanged += ListenMatchingStatus;
                await UniTask.WaitUntil(() => matchingFlag);
                //マッチングが完了次第、マッチングルームからゲームルーム番号を削除して値を返す
                gameRoom = gameRoomNumber;
                await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(null); //マッチング完了後はmatchinroomからキーを削除
                return int.Parse(gameRoom);
            }
            });
        }
        catch (MatchingFaultException){
            return 3;
        }
    }

    public async UniTask<string> CheckMatchingRoom(){
        try{
            //ルームの値がfalse or trueかの判断必要
            return await reference.Child(GetKey.MatchingRoomKey).OrderByValue().LimitToFirst(1).GetValueAsync().ContinueWith(task => {
                gameRoomNumber = task.Result.GetRawJsonValue().Substring(2, 8); //キー名が確定しない構造でjsonutilityで変換できないので一旦文字列の切り出しでgameroomの取り出しをする
                return gameRoomNumber;
            });
        }
        catch {
            return null;
        }          
    }

    public void ListenMatchingStatus(object sender, ValueChangedEventArgs args){
        if(args.DatabaseError != null){
            return;
        }
        if(Convert.ToBoolean(args.Snapshot.GetRawJsonValue()) == true){
            matchingFlag = true;
            reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).ValueChanged -= ListenMatchingStatus;
        }
    }

    public string MakeMathingRoomNumber(){
        //現時刻をゲームルームとして利用
        //極力かぶりが発生しないようミリ秒単位まで含める
        DateTime dateTime = DateTime.Now;
        string gameRoomString = String.Format("{0: HHmmssfff}", dateTime);
        return gameRoomString;
    }
}