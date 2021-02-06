using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoTransparentPanel : MonoBehaviour
{
    [SerializeField] GoTransparentButton[] goTransparentButtons;
    [SerializeField] BoardController boardController;

    void Start(){
        boardController.boardUpdated += ResetTransparent;
    }

    private void ResetTransparent(){
        foreach(GoTransparentButton btn in goTransparentButtons){
            btn.ResetTransparent();
        }
    }
}
