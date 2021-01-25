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
    private int testIndexX = 1;
    private int testIndexZ = 1;

    void Start(){
        myColor = gameController.Rival;
    }
    async void Update(){
        if(myColor == gameController.CurrentTurn & !thinking){
            thinking = true;
            await UniTask.Delay(3000);
            board.AddGo(testIndexX, testIndexZ, 2);
            testIndexX += 1;
            testIndexZ += 1;
            if(testIndexX >3){
                testIndexX -= 1;
                testIndexZ -= 1;
            }
            thinking = false;
        }
    }
}
