using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;
using FirebaseChildKey;
using CustomException;

//各処理をうまく分離して運用できる方法を検討
//現状1つのファイルの1つのクラスに処理をまとめる or 各処理ごとにGameObjectを配置するかの2案
interface ISetUserName{
    UniTask SetUserName(string setUserName);
}

interface IUserNameValidation{
    UniTask<bool> UserNameValidation(string userName);
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

//
public class ConnectFirebase:MonoBehaviour, ISetUserName, IUserNameValidation, ISetRecord, IGetRecord
{
    DatabaseReference reference;
    void Start(){
        //Start or Awakeメソッド内でreference作らないといけない
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //入力したユーザー名が既にFirabase上にないか確認する
    public async UniTask<bool> UserNameValidation(string userName){
        try{
            return await reference.Child("user").Child(userName).GetValueAsync().ContinueWith(task => {
                if(task.Result.GetRawJsonValue() == null){
                    return true;
                }
                else{
                    return false;
                }
            });
        }
        catch{
            return false;
        }
    }

    //ユーザー名をFirebaseへ格納する
    public async UniTask SetUserName(string setUserName){
        await reference.Child(GetKey.UserKey).Child(setUserName).SetValueAsync(true);
    }

    //勝敗レコードをFirebaseへ格納する
    public async UniTask SetRecord(string userName, int winCount, int loseCount){
        GameRecord gameRecord = new GameRecord(winCount, loseCount);
        string json = JsonUtility.ToJson(gameRecord);
        await reference.Child(GetKey.RecordKey).Child(userName).SetRawJsonValueAsync(json);
    }

    //勝敗レコードをFirebaseから取得する
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
            //何らかの理由でレコードがなかった場合、現状の対応としては勝敗共に0で初期化する
            await SetRecord(userName, 0, 0);
            return await GetRecord(userName);
        }
    }

    public bool matchingFlag = false; //マッチング待機するためのフラグ
    public string gameRoomNumber = null;
    //マッチング処理
    public async UniTask<int> Matching(){
        try{
            return await UniTask.Run(async () => {
                gameRoomNumber = await CheckMatchingRoom();
            }).ContinueWith(async () => {
                if(gameRoomNumber != null){
                    await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(true);
                    return int.Parse(gameRoomNumber);
                }
                else{
                    gameRoomNumber = MakeMathingRoomNumber(); //ゲームルームの番号を発行する
                    //値falseで初期セットしておき、trueになるまで待機する
                    await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(false);
                    reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).ValueChanged += ListenMatchingStatus;
                    await UniTask.WaitUntil(() => matchingFlag);

                    //マッチングが完了次第、マッチングルームからゲームルーム番号を削除して値を返す
                    await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(null); //マッチング完了後はmatchinroomからキーを削除
                    return int.Parse(gameRoomNumber);
                }
            });
        }
        catch {
            //なんらかの理由でマッチングに失敗した場合は0を返して処理を終了する
            return 0;
        }
    }

    //待機ユーザーがいるかどうかの確認
    public async UniTask<string> CheckMatchingRoom(){
        string roomNumber = null;
        try{
            //昇順に並べ替えて最初のレコードを取得してくる
            //下記返却値
            //・待機ユーザーがいない場合->null
            //・待機ユーザーがいる場合->ルーム番号
            await reference.Child(GetKey.MatchingRoomKey).OrderByKey().LimitToFirst(1).GetValueAsync().ContinueWith(task => {
                roomNumber = task.Result.GetRawJsonValue().Substring(2, 8); //キー名が確定しない構造でjsonutilityで変換できないので文字列の切り出しでgameroomの取り出しをする
            });
        }
        catch {
        }
        return roomNumber;      
    }

    //作成した待機ルームの更新(マッチング)を待つ
    public void ListenMatchingStatus(object sender, ValueChangedEventArgs args){
        if(args.DatabaseError != null){
            //エラーが発生した場合は読み取りを終える
            return;
        }
        if(Convert.ToBoolean(args.Snapshot.GetRawJsonValue()) == true){
            matchingFlag = true;
            reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).ValueChanged -= ListenMatchingStatus;
        }
    }

    //現時刻からルーム番号を生成するメソッド
    //極力かぶりが発生しないようミリ秒単位まで含めて時間、分、秒、ミリ秒の値を繋げて番号を作成する
    public string MakeMathingRoomNumber(){
        DateTime dateTime = DateTime.Now;
        string gameRoomString = String.Format("{0:HHmmssff}", dateTime);
        return gameRoomString;
    }
}