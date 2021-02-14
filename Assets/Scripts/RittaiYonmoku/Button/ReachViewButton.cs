using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
public class ReachViewButton : MonoBehaviour
{
    public delegate void ReachDisplaySwitchEventHandler();
    public event ReachDisplaySwitchEventHandler switchEvent = () => {};
    [SerializeField] Text reachOnText;
    [SerializeField] Text reachOffText;
    [SerializeField] PlaySE playSE;
    private bool currentFlag;
    public bool CurrentFlag{get {return currentFlag;}}

    void Start(){
        currentFlag = true;
        reachOnText.enabled = true;
        reachOffText.enabled = false;
    }

    public void OnClicked(){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        SwitchText(currentFlag);
        switchEvent();
    }

    private void SwitchText(bool status){
        currentFlag = !status;
        reachOnText.enabled = !status;
        reachOffText.enabled = status;
    }
}
