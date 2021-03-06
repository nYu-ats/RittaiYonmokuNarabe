﻿using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

/*
インスペクター上ではX、Z座標を連結した文字列を各ボタンの名前として設定
*/

public class PutPositionPanel : MonoBehaviour
{
    //各棒のアウトラインを更新するためのイベント
    public delegate void PanelUpdatedEventHandler(int x, int z);
    public event PanelUpdatedEventHandler panelUpdated = (int x, int z) => {};

    [SerializeField] Image CurosrImg;
    [SerializeField] CameraMover cameraMover;
    [SerializeField] PutPositionButton[] putPositionButton;
    [SerializeField] BoardController boardController;
    private (int x, int z) indexXZ = (0, 0);
    public (int x, int z) IndexXZ{get {return indexXZ;}}

    public void Start(){
        //各ボタンクリック時に呼び出されるイベントのハンドラー追加
        foreach(PutPositionButton btn in putPositionButton){
            btn.selectedStatusUpdate += SelectedPositionUpdate;
        }
        SelectedPositionUpdate(indexXZ.x, indexXZ.z);
    }
    void Update(){
        //カメラ視点の回転に追従して回転するようにする
        this.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, cameraMover.TmpPhi);
    }

    public void SelectedPositionUpdate(int indexX, int indexZ){
        if(boardController.CheckCanPut(indexX, indexZ) == BoardStatus.CanNotPut){
            //すでに4つの碁が置かれている座標は選択できないようにする
            return;
        }
        else{
            indexXZ.x = indexX;
            indexXZ.z = indexZ;
            Transform selectedPos = this.gameObject.transform.Find(indexX.ToString() + indexZ.ToString());
            CurosrImg.transform.position = selectedPos.position;
            panelUpdated(indexXZ.x, indexXZ.z);
        }
    }
}
