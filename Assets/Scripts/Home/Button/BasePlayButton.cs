using UnityEngine;
using UnityEngine.SceneManagement;
using CommonConfig;

public class BasePlayButton : MonoBehaviour
{
    protected int loadPlayMode = 0;
    protected int gameRoom = 0;
    protected int playerColor = 0;
    protected int rivalColor = 0;

    //ゲーム本体のシーンが読み込まれたときのプレイモードの設定をする
    //加えてゲームシーン->ホームシーンの時には呼び出す必要がないのでイベントハンドラーから消去しておく
    protected void SetGameVariable(Scene loadScene, LoadSceneMode mode){
        GameController gameController = GameObject.FindWithTag(Tags.InRittaiYonmoku.GameController).GetComponent<GameController>();
        gameController.PlayMode = loadPlayMode;
        gameController.Player = playerColor;
        gameController.Rival = rivalColor;
        gameController.GameRoom = gameRoom;
        SceneManager.sceneLoaded -= SetGameVariable;
        }
    }
