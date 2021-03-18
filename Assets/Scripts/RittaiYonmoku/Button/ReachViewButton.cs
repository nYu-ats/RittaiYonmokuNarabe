using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
public class ReachViewButton : MonoBehaviour
{
    //ターンに関わらず任意のタイミングでon/offできるようにするため
    //切替ボタンが押されたらeventを発生させる
    public delegate void ReachDisplaySwitchEventHandler();
    public event ReachDisplaySwitchEventHandler switchEvent = () => {};
    [SerializeField] Image reachOnText;
    [SerializeField] Image reachOffText;
    [SerializeField] PlaySE playSE;
    private bool currentFlag;
    public bool CurrentFlag{get {return currentFlag;}}

    void Start(){
        currentFlag = true;
        reachOnText.enabled = currentFlag;
        reachOffText.enabled = false;
    }

    public void OnClicked(){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        SwitchText(currentFlag);
        switchEvent();
    }

    private void SwitchText(bool status){
        //リーチ表示のon/offが切り替わったらText自体を差し替える
        currentFlag = !status;
        reachOnText.enabled = !status;
        reachOffText.enabled = status;
    }
}
