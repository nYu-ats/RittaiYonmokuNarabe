using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
X、Z座標を並べた文字列を各ボタンの名前として設定している
*/

public class PutPositionPanel : MonoBehaviour
{
    [SerializeField] Image CurosrImg;
    [SerializeField] CameraMover cameraMover;
    [SerializeField] PutPositionButton[] putPositionButton;
    private (int x, int z) indexXZ = (0, 0);
    public (int x, int z) IndexXZ{get {return indexXZ;}} //置くボタンからアクセスするためのプロパティ

    public void Start(){
        foreach(PutPositionButton btn in putPositionButton){
            btn.selectedStatusUpdate += SelectedPositionUpdate;
        }
        SelectedPositionUpdate(indexXZ.x, indexXZ.z);
    }
    void Update(){
        this.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, cameraMover.TmpPhi);
    }

    public void SelectedPositionUpdate(int indexX, int indexZ){
        indexXZ.x = indexX;
        indexXZ.z = indexZ;
        Transform selectedPos = this.gameObject.transform.Find(indexX.ToString() + indexZ.ToString());
        CurosrImg.transform.position = selectedPos.position;
    }
}
