using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using FirebaseChildKey;
using CustomException;
using CommonConfig;
using System.Text.RegularExpressions;

interface ISetGo{
    UniTask SetGo((int x, int z, int y, int color) updateInfo);
}

interface IWaitRivalAction{
    UniTask<(int x, int z, int y, int color)> WaitRivalAction();
}

public class FirebaseUpdateBoardFunc : BaseFirebaseFunc, ISetGo, IWaitRivalAction
{
    [SerializeField] GameController gameController;
    [SerializeField] BoardController boardController;
    private bool gameUpdated = false;
    private (int x, int z, int y, int color) rivalAction;

    public async UniTask SetGo((int x, int z, int y, int color) updateInfo){
        string setKey = updateInfo.x.ToString() + updateInfo.z.ToString() + updateInfo.y.ToString();
        await reference.Child(gameController.GameRoom.ToString()).Child(GetKey.GameStatus).Child(setKey).SetValueAsync(updateInfo.color.ToString());
    }

    public async UniTask<(int x, int z, int y, int color)> WaitRivalAction(){
        try{
            reference.Child(gameController.GameRoom.ToString()).Child(GetKey.GameStatus).ValueChanged += ListenGameStatusUpdate;
            await UniTask.WaitUntil(() => gameUpdated).ContinueWith(() => {
                reference.Child(gameController.GameRoom.ToString()).Child(GetKey.GameStatus).ValueChanged -= ListenGameStatusUpdate;
            }); //相手のTimeOutがあるのでこちらでTimeOutを設定する必要はない
            gameUpdated = false;
            if(rivalAction.x == GameRule.GiveUpSignal){
                throw new GiveUpSignalReceive();
            }
            return rivalAction;
        }
        catch (GiveUpSignalReceive){
            //例外再スロー
            throw;
        }
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
                if(boardController.CheckCanPut(x, z) == y | x == GameRule.GiveUpSignal){
                    rivalAction = (x, z, y, gameController.Rival);
                    gameUpdated = true;
                }
            }
        }
    }
}
