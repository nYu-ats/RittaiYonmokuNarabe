using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToResultButton : MonoBehaviour
{
    [SerializeField] GameObject gameResultPanel;
    [SerializeField] GameObject giveUpConfirmPanel;
    [SerializeField] GameController gameController;

    public void OnClicked(){
        //ギブアップは自身のターンにしかできないため、ゲームリザルトに相手を勝者としてセットする
        gameController.CurrentTurn += gameController.Rival;
        gameResultPanel.SetActive(true);
        giveUpConfirmPanel.SetActive(false);
    }
}
