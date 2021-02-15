using System;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using FirebaseChildKey;
using CommonConfig;
using System.Text.RegularExpressions;

interface IMatching{
    UniTask<(int roomNumber, int matchingPattern)> Matching();
}

public class FirebaseMatchingFunc : BaseFirebaseFunc, IMatching
{
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
                await UniTask.WaitUntil(() => matchingFlag).Timeout(TimeSpan.FromSeconds(30));
                await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(null); //マッチング完了後はmatchinroomからキーを削除
                return (int.Parse(gameRoomNumber), MatchingPattern.CreateRoom);
            }
        }
        catch {
            //なんらかの理由でマッチングに失敗した場合はリッスンを中断する
            reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).ValueChanged -= ListenMatchingStatus;
            throw;
        }
        finally{
            //マッチング成功/失敗に関わらず、マッチング処理終了時にゲームルーム番号を削除して値を返す
            await reference.Child(GetKey.MatchingRoomKey).Child(gameRoomNumber).SetValueAsync(null);  
        }
    }

    //待機ユーザーがいるかどうかの確認
    private async UniTask<string> CheckMatchingRoom(){
        //・待機ユーザーがいない場合->null
        //・待機ユーザーがいる場合->ルーム番号
        string existRoomNumber = null;
        //realtime database内で昇順に並んでいるので最初の(作成日時が一番早い)ゲームルームを取得する
        await reference.Child(GetKey.MatchingRoomKey).GetValueAsync().ContinueWith(task => {
            if(task.Result.GetRawJsonValue() != null){
                existRoomNumber = ParseGameRoomString(task.Result.GetRawJsonValue());
            }
        });
        return existRoomNumber;   
    }

    private string ParseGameRoomString(string jsonStr){
        //キー名が固定ではなく、jsonutilityで変換できないので
        //文字列の切り出しでgameroomの取り出しをする
        string[] roomStatus = jsonStr.Split(',');
        string roomNumber = null;
        foreach(string room in roomStatus){
            if(room.Contains("false")){
                //falseの場合のみ有効なゲームルームとして取り出す
                roomNumber = Regex.Match(room,  @"\d{8}").ToString();
                break;
            }
        }
        return roomNumber;
    }

    //作成した待機ルームの更新(マッチング)を待つ
    private void ListenMatchingStatus(object sender, ValueChangedEventArgs args){
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
    private string MakeMathingRoomNumber(){
        DateTime dateTime = DateTime.Now;
        string gameRoomString = String.Format("{0:HHmmssff}", dateTime);
        return gameRoomString;
    }
}
