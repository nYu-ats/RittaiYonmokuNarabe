using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

interface ISetPlayerName{
    void SetPlayerName(int playerTurn, string rivalName);
}
public class UserNamePanel : MonoBehaviour, ISetPlayerName
{
    [SerializeField] Text whiteUser;
    [SerializeField] Text blackUser;
    private int soloPlay = 1;
    private int multiPlay = 2;

    public void SetPlayerName(int playerTurn, string rivalName){
        if(playerTurn == GameRule.FirstAttack){
            whiteUser.text = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
            blackUser.text = rivalName;
        }
        else{
            whiteUser.text = rivalName;
            blackUser.text = PlayerPrefs.GetString(PlayerPrefsKey.UserNameKey);
        }
    }
}
