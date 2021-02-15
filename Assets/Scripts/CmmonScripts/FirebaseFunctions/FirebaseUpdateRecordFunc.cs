using UnityEngine;
using Cysharp.Threading.Tasks;
using FirebaseChildKey;
using CustomException;

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

public class FirebaseUpdateRecordFunc : BaseFirebaseFunc, ISetRecord, IGetRecord
{
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
            //何らかの理由でレコードがなかった場合、現状の対応としては勝敗共に0をセットする
            await SetRecord(userName, 0, 0);
            return await GetRecord(userName);
        }
    }
}
