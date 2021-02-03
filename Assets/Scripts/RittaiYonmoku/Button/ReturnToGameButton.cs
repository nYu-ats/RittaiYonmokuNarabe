using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToGameButton : MonoBehaviour
{
    [SerializeField] GameObject giveUpConfirmPanel;

    public void OnClicked(){
        giveUpConfirmPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
