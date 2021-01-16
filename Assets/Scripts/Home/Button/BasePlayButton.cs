using UnityEngine;
using UnityEngine.SceneManagement;
using CommonConfig;

public class BasePlayButton : MonoBehaviour
{
    protected int loadPlayMode = 0;
    protected int gameRoom = 0;
    protected enum PlayMode
    {
        Solo = 1,
        Multi = 2
    }

    //ゲーム本体のシーンが読み込まれたときのプレイモードの設定をする
    //加えてゲームシーン->ホームシーンの時には呼び出す必要がないのでイベントハンドラーから消去しておく
    protected void SetPlayMode(Scene loadScene, LoadSceneMode mode){
        GameController gameController = GameObject.FindWithTag(Tags.InRittaiYonmoku.GameController).GetComponent<GameController>();
        gameController.playMode = loadPlayMode;
        //プレイモードが増えた時に分岐を追加しやすくするためswitchを使用
        switch(loadPlayMode){
            case (int)PlayMode.Solo:
            goto default;

            case (int)PlayMode.Multi:
            gameController.gameRoom = gameRoom;
            goto default;

            default:
            SceneManager.sceneLoaded -= SetPlayMode;
            break;

        }
    }
}
