using UnityEngine;
using CommonConfig;

interface IPutGo{
    void PutGo(int xIndex, int zIndex, int yIndex, int addColor);
}
public class GoGenerator : MonoBehaviour, IPutGo
{
    [SerializeField] GameObject goWhite;
    [SerializeField] GameObject goBlack;
    [SerializeField] float pointY; //碁を生成する高さ
    [SerializeField] Board board;

    //碁を生成するためのXとZ座標の設定
    private (float x, float z)[][] pointXZ = new (float, float)[][]{
        new(float, float)[]{(4.5f, 4.5f), (4.5f, 1.5f), (4.5f, -1.5f), (4.5f, -4.5f)},
        new(float, float)[]{(1.5f, 4.5f), (1.5f, 1.5f), (1.5f, -1.5f), (1.5f, -4.5f)},
        new(float, float)[]{(-1.5f, 4.5f), (-1.5f, 1.5f), (-1.5f, -1.5f), (-1.5f, -4.5f)},
        new(float, float)[]{(-4.5f, 4.5f), (-4.5f, 1.5f), (-4.5f, -1.5f), (-4.5f, -4.5f)}
        };

    public void PutGo(int xIndex, int zIndex, int yIndex, int addColor){
        //碁を盤上に出現させる処理
        float pointX = pointXZ[xIndex][zIndex].x;
        float pointZ = pointXZ[xIndex][zIndex].z;
        GameObject putGo = null;
        if(addColor ==BoardStatus.GoWhite){
            putGo = Instantiate(goWhite, new Vector3(pointX, pointY, pointZ), Quaternion.identity);

        }
        else if(addColor ==BoardStatus.GoBlack){
            putGo = Instantiate(goBlack, new Vector3(pointX, pointY, pointZ), Quaternion.identity);       
        }
        putGo.GetComponent<Go>().ThisPosition = (xIndex, zIndex, yIndex);
        putGo.transform.parent = board.transform;
    }
}
