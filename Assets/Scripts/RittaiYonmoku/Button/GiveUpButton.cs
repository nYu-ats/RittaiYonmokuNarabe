using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveUpButton : MonoBehaviour
{
    [SerializeField] GameObject giveUpConfirmPanel;

    public void OnClicked(){
        giveUpConfirmPanel.SetActive(true);
        Time.timeScale = 0;
    }
}
