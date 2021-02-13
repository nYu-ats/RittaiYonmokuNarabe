using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using cakeslice;
using CommonConfig;

public class Go : MonoBehaviour
{
    private ReachChecker reachChecker;
    private (int x, int z, int y) thisPosition;
    public (int x, int z, int y) ThisPosition{set {thisPosition = value;}}


    void Start(){
        reachChecker = GameObject.FindWithTag(Tags.InRittaiYonmoku.ReachChecker).GetComponent<ReachChecker>();
        reachChecker.hasReach += DisplayOutLine;
        DisplayOutLine(reachChecker.ReachPositions);
    }

    private void DisplayOutLine((int x, int z, int y)[] reachLines){
        if(reachChecker.ReachDisplayFlag){
            if(reachLines.Contains(thisPosition)){
                this.GetComponent<Outline>().color = 0;
            }
            else{
                this.GetComponent<Outline>().color = 2;
            }
        }
        else{
            this.GetComponent<Outline>().color = 2;
        }
    }
}
