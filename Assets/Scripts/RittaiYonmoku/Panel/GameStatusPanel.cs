using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GameStatusPanel : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] GameObject checkMateUI;
    [SerializeField] GameObject gameResultPanel;
    [SerializeField] int checkMateDisplayTime = 3000;
    void Start()
    {
        gameController.checkMateEvent += DisplayCheckMate;
    }

    async private void DisplayCheckMate(){
        checkMateUI.SetActive(true);
        await UniTask.Delay(checkMateDisplayTime)
        .ContinueWith(() => gameResultPanel.SetActive(true)); //ゲームリザルトを表示するまで数秒間をあける
        gameController.checkMateEvent -= DisplayCheckMate;
    }

}
