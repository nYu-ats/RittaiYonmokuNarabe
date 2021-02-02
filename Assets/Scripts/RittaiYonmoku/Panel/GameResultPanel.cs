using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class GameResultPanel : MonoBehaviour
{
    [SerializeField] Text winnerText;
    [SerializeField] Text numberOfMove;
    [SerializeField] GameController gameController;
    void Start()
    {
        int winner = gameController.CurrentTurn;
        int restGoNumber = gameController.GoNumber;
        if(winner == BoardStatus.GoWhite){
            winnerText.text = "白の勝利!!";
        }
        else{
            winnerText.text = "黒の勝利!!";
        }
        numberOfMove.text = (GameRule.TotalGoNumber - restGoNumber).ToString();
    }
}
