using UnityEngine;
using CommonConfig;

public class PutPositionButton : MonoBehaviour
{
    public delegate void SelectedPointChangeEventHandler(int indexX, int indexZ);
    public event SelectedPointChangeEventHandler selectedStatusUpdate = (int xIndex, int zIndex) => {};
    [SerializeField] PlaySE playSE;

    public void OnClickedAction(string indexXZ){
        playSE.PlaySound(AudioConfig.GoPositionMoveButtonIndex);
        //ボタンクリックの引数を1つしか渡せないため、文字列で受け取ってそこからXとZに分解するようにする
        int indexX = int.Parse(indexXZ.Substring(0, 1));
        int indexZ = int.Parse(indexXZ.Substring(1, 1));
        selectedStatusUpdate(indexX, indexZ);
    }
}
