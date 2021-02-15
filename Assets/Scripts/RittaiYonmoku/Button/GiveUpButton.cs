using UnityEngine;
using CommonConfig;

public class GiveUpButton : MonoBehaviour
{
    [SerializeField] GameObject giveUpConfirmPanel;
    [SerializeField] GameController gameController;
    [SerializeField] PlaySE playSE;

    public void OnClicked(){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        giveUpConfirmPanel.SetActive(true);
        if(gameController.PlayMode == GameRule.SoloPlayMode){
            Time.timeScale = 0;
        }
        //マルチプレイの場合は相手との時間制限の同期がとれなくなるので
        //ギブアップ確認のパネルを表示している間の制限時間停止は行わない
    }
}
