using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using FirebaseChildKey;
using CommonConfig;

interface ISetGameRoom{
    UniTask SetGameRoom(int roomNumber, int player);
}

interface IGetRivalName{
    UniTask<string> GetRivalName(int gameRoom, int rival);
}

public class FirebaseSetUpGameFunc : BaseFirebaseFunc, ISetGameRoom, IGetRivalName
{
    private string rivalName = null;
    private bool gameRoomCrated = false;

    public async UniTask SetGameRoom(int roomNumber, int player){
        await reference.Child(roomNumber.ToString()).Child(GetKey.GamePlayer).Child(player.ToString())
              .SetValueAsync(PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey));
    }

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
}
