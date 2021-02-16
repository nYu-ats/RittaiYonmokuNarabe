using UnityEngine;
using Cysharp.Threading.Tasks;

interface IDeleteGamerRoom{
    UniTask DeleteGameRoom();
}

public class FirebaseCloseGameFunc : BaseFirebaseFunc, IDeleteGamerRoom
{
    [SerializeField] GameController gameController;
    public async UniTask DeleteGameRoom(){
        try{
            await reference.Child(gameController.GameRoom.ToString()).SetValueAsync(null);
        }
        catch{
            throw;
        }
    }
}
