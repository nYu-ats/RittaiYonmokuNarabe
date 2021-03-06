﻿using System;
using UnityEngine;

public class ReachChecker : MonoBehaviour
{
    public delegate void HasReachEventHandler((int x, int z, int y)[] reachLines);
    public event HasReachEventHandler hasReach = ((int x, int z, int y)[] lines) => {};
    [SerializeField] BoardController boardController;
    [SerializeField] ReachViewButton reachViewButton;
    private (int x, int z, int y)[] reachPositions = new (int x, int z, int y)[]{};
    public (int x, int z, int y)[] ReachPositions{get {return reachPositions;}}
    private bool reachDisplayFlag = true;

    public bool ReachDisplayFlag{get{return reachDisplayFlag;}}

    void Start(){
        boardController.boardUpdated += UpdateReachLine;
        reachViewButton.switchEvent += SwitchReachDisplay;
    }

    private void UpdateReachLine(){
        Array.Resize(ref reachPositions, 0);
        GoSituations[] tmpReachLines = boardController.HasLines(3);
        if(tmpReachLines != null){
            foreach(GoSituations go in tmpReachLines){
                //次の手でチェックメイトとなりうる場合のみリーチとして画面に表示する
                bool reachFlag = go.RestPos[0].y == boardController.CheckCanPut(go.RestPos[0].x, go.RestPos[0].z);
                if(reachFlag){
                    foreach((int x, int z, int y) reachPos in go.Positions){
                        Array.Resize(ref reachPositions, reachPositions.Length + 1);
                        reachPositions[reachPositions.Length - 1] = reachPos;
                    }
                }
            }
        }
        hasReach(reachPositions);
    }

    private void SwitchReachDisplay(){
        reachDisplayFlag = reachViewButton.CurrentFlag;
        hasReach(reachPositions);
    }
}
