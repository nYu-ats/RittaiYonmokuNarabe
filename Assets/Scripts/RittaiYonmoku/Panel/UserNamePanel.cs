using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
public class UserNamePanel : MonoBehaviour
{
    [SerializeField] Text whiteUser;
    [SerializeField] Text blackUser;
    [SerializeField] GameController gameController;
    private int soloPlay = 1;
    private int multiPlay = 2;
    private string NPCName = "NPC";


    void Start(){
        SetPlayerName(gameController.PlayMode);
    }

    private void SetPlayerName(int playMode){
        //ソロプレイモードの場合は相手の名前はNPCで固定
        //マルチプレイの場合はgameRoomから対戦相手の名前を取得する
        if(playMode == soloPlay){
            if(gameController.Player == GameRule.FirstAttack){
                whiteUser.text = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
                blackUser.text = NPCName;
            }
            else{
                whiteUser.text = NPCName;
                blackUser.text = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
            }
        }
    }
}
