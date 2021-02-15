using UnityEngine;
using CommonConfig;
public class ToResultButton : MonoBehaviour
{
    [SerializeField] GameObject gameResultPanel;
    [SerializeField] GameObject giveUpConfirmPanel;
    [SerializeField] GameController gameController;
    [SerializeField] SyncBoardStatus syncBoardStatus;
    [SerializeField] PlaySE playSE;

    public void OnClicked(){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        //ゲームリザルトにはCrrentTurnのユーザーを勝者として表示するようになっており
        //ギブアップは自身のターンにしかできないため、CurrentTurnを相手に更新する
        gameController.CurrentTurn += gameController.Rival;
        gameResultPanel.SetActive(true);
        giveUpConfirmPanel.SetActive(false);
        if(gameController.PlayMode == GameRule.MultiPlayMode){
            //マルチプレイの場合は相手にギブアップを通知をする
            syncBoardStatus.GiveUpAction();
        }
    }
}
