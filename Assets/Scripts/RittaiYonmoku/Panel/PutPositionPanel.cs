using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

/*
インスペクター上ではX、Z座標を並べた文字列を各ボタンの名前として設定
*/

public class PutPositionPanel : MonoBehaviour
{
    [SerializeField] Image CurosrImg;
    [SerializeField] CameraMover cameraMover;
    [SerializeField] PutPositionButton[] putPositionButton;
    [SerializeField] BoardController boardController;
    private (int x, int z) indexXZ = (0, 0);
    public (int x, int z) IndexXZ{get {return indexXZ;}} //置くボタンからアクセスするためのプロパティ

    public void Start(){
        //各ボタンにクリック時の呼び出されるイベントのハンドラー追加
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
            //すでに4つの碁が置かれている棒には移動できないようにする
            return;
        }
        else{
            indexXZ.x = indexX;
            indexXZ.z = indexZ;
            Transform selectedPos = this.gameObject.transform.Find(indexX.ToString() + indexZ.ToString());
            CurosrImg.transform.position = selectedPos.position;
        }
    }
}
