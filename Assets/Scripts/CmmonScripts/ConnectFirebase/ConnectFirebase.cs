using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;
using FirebaseChildKey;
using CustomException;
using CommonConfig;
using System.Text.RegularExpressions;

//各処理をうまく分離して運用できる方法を検討
//現状1つのファイルの1つのクラスに処理をまとめる or 各処理ごとにGameObjectを配置するかの2案
interface ISetUserName{
    UniTask SetUserName(string setUserName);
}

interface IUserNameValidation{
    UniTask<bool> UserNameValidation(string userName);
}

interface IMatching{
    (int roomNumber, int matchingPatten) Matching(string userName);
}

interface ISetRecord{
    UniTask SetRecord(string userName, int winCount, int loseCount);
}

interface IGetRecord{
    UniTask<GameRecord> GetRecord(string userName);
}

interface ISetGameRoom{
    UniTask SetGameRoom(int roomNumber, int player);
}

interface IGetRivalName{
    UniTask<string> GetRivalName(int gameRoom, int rival);
}

interface IAddGoFirebase{
    UniTask AddGoFirebase();
}

interface ISetGo{
    UniTask SetGo((int x, int z, int y, int color) updateInfo);
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

public class ConnectFirebase:MonoBehaviour, ISetUserName, IUserNameValidation, ISetRecord, IGetRecord, ISetGameRoom, IGetRivalName, ISetGo
{
    DatabaseReference reference;
    void Start(){
        //起動時にでreference作らないといけない
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
    public async UniTask<(int roomNumber, int matchingPattern)> Matching(){
        try{
            gameRoomNumber = await CheckMatchingRoom();
            if(gameRoomNumber != null){
                await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(true);
                return (int.Parse(gameRoomNumber), MatchingPattern.EnterRoom);
            }
            else{
                gameRoomNumber = MakeMathingRoomNumber(); //ゲームルームの番号を発行する
                //値falseで初期セットしておき、trueになるまで待機する
                await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(false);
                reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).ValueChanged += ListenMatchingStatus;
                await UniTask.WaitUntil(() => matchingFlag);

                //マッチングが完了次第、マッチングルームからゲームルーム番号を削除して値を返す
                await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(null); //マッチング完了後はmatchinroomからキーを削除
                return (int.Parse(gameRoomNumber), MatchingPattern.CreateRoom);
            }
        }
        catch {
            //なんらかの理由でマッチングに失敗した場合は0を返して処理を終了する
            return (0, 0);
        }
    }

    //待機ユーザーがいるかどうかの確認
    public async UniTask<string> CheckMatchingRoom(){
        //・待機ユーザーがいない場合->null
        //・待機ユーザーがいる場合->ルーム番号
        string existRoomNumber = null;
            //昇順に並べ替えて最初のレコードを取得してくる
            await reference.Child(GetKey.MatchingRoomKey).OrderByKey().LimitToFirst(1).GetValueAsync().ContinueWith(task => {
                if(task.Result.GetRawJsonValue() != null){
                    existRoomNumber = task.Result.GetRawJsonValue().Substring(2, 8); //キー名が固定ではなく、jsonutilityで変換できないので文字列の切り出しでgameroomの取り出しをする
                }
            });
        return null; //テストのため一旦null   
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

    public async UniTask SetGameRoom(int roomNumber, int player){
        await reference.Child(roomNumber.ToString()).Child(GetKey.GamePlayer).Child(player.ToString())
              .SetValueAsync(PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey));
    }

    string rivalName = null;
    public async UniTask<string> GetRivalName(int roomNumber, int rival){
        await reference.Child(roomNumber.ToString()).Child(GetKey.GamePlayer).Child(rival.ToString()).GetValueAsync().ContinueWith(task => {
            if(task.Result.Exists){
                rivalName = task.Result.GetRawJsonValue();
            }
        });
        if(rivalName == null){
            reference.Child(roomNumber.ToString()).Child(GetKey.GamePlayer).Child(rival.ToString()).ValueChanged += ListenGameRoomStatus;
            await UniTask.WaitUntil(() => gameRoomCrated).ContinueWith(() => {
                reference.Child(roomNumber.ToString()).Child(GetKey.GamePlayer).Child(rival.ToString()).ValueChanged -= ListenGameRoomStatus;
            }).Timeout(TimeSpan.FromSeconds(30));
        }
        rivalName = rivalName.Trim('\"');
        return rivalName;
    }

    private bool gameRoomCrated = false;

    private void ListenGameRoomStatus(object sender, ValueChangedEventArgs args){
        if(args.DatabaseError != null){
            //エラーが発生した場合は読み取りを終える
            return;
        }
        if(args.Snapshot.GetRawJsonValue() != null){
            rivalName = args.Snapshot.GetRawJsonValue();
            gameRoomCrated = true;
        }
    }

    [SerializeField] GameController gameController;
    [SerializeField] BoardController boardController;
    private bool gameUpdated = false;
    private (int x, int z, int y, int color) rivalAction;

    public async UniTask SetGo((int x, int z, int y, int color) updateInfo){
        string setKey = updateInfo.x.ToString() + updateInfo.z.ToString() + updateInfo.y.ToString();
        await reference.Child(gameController.GameRoom.ToString()).Child(GetKey.GameStatus).Child(setKey).SetValueAsync(updateInfo.color.ToString());
    }

    public async UniTask<(int x, int z, int y, int color)> WaitRivalAction(){
        Debug.Log("liseten start");
        reference.Child(gameController.GameRoom.ToString()).Child(GetKey.GameStatus).ValueChanged += ListenGameStatusUpdate;
            await UniTask.WaitUntil(() => gameUpdated).ContinueWith(() => {
                reference.Child(gameController.GameRoom.ToString()).Child(GetKey.GameStatus).ValueChanged -= ListenGameStatusUpdate;
            }).Timeout(TimeSpan.FromSeconds(90));
        gameUpdated = false;
        return rivalAction;
    } 

    private void ListenGameStatusUpdate(object sender, ValueChangedEventArgs args){
        if(args.DatabaseError != null){
            //エラーが発生した場合は読み取りを終える
            return;
        }
        if(args.Snapshot.GetRawJsonValue() != null){
            MatchCollection positions = Regex.Matches(args.Snapshot.GetRawJsonValue().ToString(), @"\d{3}");
            foreach(Match str in positions){
                int x = int.Parse(str.Value[0].ToString());
                int z = int.Parse(str.Value[1].ToString());
                int y = int.Parse(str.Value[2].ToString());
                if(boardController.CheckCanPut(x, z) == y){
                    rivalAction = (x, z, y, gameController.Rival);
                    gameUpdated = true;
                }
            }
        }
    }
}