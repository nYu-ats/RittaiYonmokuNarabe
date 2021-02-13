using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReachViewButton : MonoBehaviour
{
    public delegate void ReachDisplaySwitchEventHandler();
    public event ReachDisplaySwitchEventHandler switchEvent = () => {};
    [SerializeField] Text reachOnText;
    [SerializeField] Text reachOffText;
    private bool currentFlag;
    public bool CurrentFlag{get {return currentFlag;}}

    void Start(){
        currentFlag = true;
        reachOnText.enabled = true;
        reachOffText.enabled = false;
    }

    public void OnClicked(){
        SwitchText(currentFlag);
        switchEvent();
    }

    private void SwitchText(bool status){
        currentFlag = !status;
        reachOnText.enabled = !status;
        reachOffText.enabled = status;
    }
}
