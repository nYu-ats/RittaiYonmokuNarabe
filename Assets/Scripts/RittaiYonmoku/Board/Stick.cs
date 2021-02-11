using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Stick : MonoBehaviour
{
    [SerializeField] PutPositionPanel putPositionPanel;
    private Outline outline;

    void Start(){
        putPositionPanel.panelUpdated += UpdateSelectedPos;
        outline = this.GetComponent<Outline>();
    }

    private void UpdateSelectedPos(int x, int z){
        string objectedStick = "bou" + x.ToString() + z.ToString();
        if(this.gameObject.name == objectedStick){
            outline.color = 1;
        }
        else{
            outline.color = 2;
        }
    }
}