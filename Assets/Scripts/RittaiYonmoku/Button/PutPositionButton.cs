using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutPositionButton : MonoBehaviour
{
    public delegate void SelectedPointChangeEventHandler(int indexX, int indexZ);
    public event SelectedPointChangeEventHandler selectedStatusUpdate = (int xIndex, int zIndex) => {};
    public void OnClickedAction(string indexXZ){
        //ボタンクリックの引数を1つしか渡せないため文字列で受け取って、そこからXとZに分解するようにする
        int indexX = int.Parse(indexXZ.Substring(0, 1));
        int indexZ = int.Parse(indexXZ.Substring(1, 1));
        selectedStatusUpdate(indexX, indexZ);
    }
}
