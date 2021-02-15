using UnityEngine;
using Cysharp.Threading.Tasks;

interface IDeleteGamerRoom{
    UniTask DeleteGameRoom();
}

public class FirebaseCloseGameFunc : BaseFirebaseFunc, IDeleteGamerRoom
{
    [SerializeField] GameController gameController;
    public async UniTask DeleteGameRoom(){
        await reference.Child(gameController.GameRoom.ToString()).SetValueAsync(null);
    }    
}
