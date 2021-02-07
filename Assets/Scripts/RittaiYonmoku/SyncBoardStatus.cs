using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;
using Cysharp.Threading.Tasks;

public class SyncBoardStatus : MonoBehaviour
{
    [SerializeField] BoardController boardController;
    [SerializeField] GameController gameController;
    [SerializeField] ConnectFirebase connectFirebase;    
    void Start(){
        if(gameController.Player == GameRule.FirstAttack){
            boardController.boardUpdated += SetGo;
        }
    }

    private void SetGo(){
        UniTask.Create(async () => {
            await connectFirebase.SetGo(boardController.LastUpdate);
            boardController.boardUpdated -= SetGo; //相手のボード更新時に呼び出せれないようにする
            //await connectFirebase.WaitRivalAction();
        }).Forget();
        UniTask.Create(async () => {
            (int x, int z, int y, int color) rivalAction = await connectFirebase.WaitRivalAction();
            Debug.Log("rival put");
            boardController.AddGo(rivalAction.x, rivalAction.z, rivalAction.color);
            boardController.boardUpdated += SetGo;
        }).Forget();
    }
}
