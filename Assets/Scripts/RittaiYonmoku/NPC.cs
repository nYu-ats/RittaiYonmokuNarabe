using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NPC : MonoBehaviour
{
    [SerializeField] Board board;
    [SerializeField] GameController gameController;
    private string myColor;
    private bool thinking = false;

    void Start(){
        myColor = gameController.Rival;
    }
    async void Update(){
        if(myColor == gameController.CurrentTurn & !thinking){
            thinking = true;
            await UniTask.Delay(3000);
            board.AddGo(1, 1, 2);
            thinking = false;
        }
    }
}
