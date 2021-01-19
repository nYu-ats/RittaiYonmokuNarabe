using UnityEngine;

public class GoGeneratorHome : MonoBehaviour
{
    [SerializeField] GameObject[] go;
    [SerializeField] float generateInterbal = 3.0f;
    //碁をランダム生成する座標の範囲
    //Y座標は固定
    //XとZはおいおい変更する可能性があり、厳密に小数点で指定する必要がないため列挙子を使う
    enum GoGeneratePositionRangeX{
        Start = -5,
        End = 5
    }

    enum GoGeneratePositionRangeZ{
        Start = 5,
        End = 10
    }

    [SerializeField] int goGeneratePositionY = 25;

    private float time;

    enum ColorFlag{
        White = 0,
        Black = 1
    }

    void Start(){
        time = 0.0f;
    }

    void Update(){
        time += Time.deltaTime;
        if(time > generateInterbal){
            float nextGeneratePositionX = Random.Range((float)((int)GoGeneratePositionRangeX.Start), (float)((int)GoGeneratePositionRangeX.End));
            float nextGeneratePositionZ = Random.Range((float)((int)GoGeneratePositionRangeZ.Start), (float)((int)GoGeneratePositionRangeZ.End));
            int nextGenerateColor = Random.Range(0,2);
            Instantiate(go[nextGenerateColor], new Vector3(nextGeneratePositionX, goGeneratePositionY, nextGeneratePositionZ), Quaternion.Euler(0, 0, 0));
            time = 0.0f;
        }
    }
}
