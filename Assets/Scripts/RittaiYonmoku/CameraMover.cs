using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IRollCamera{
    //ユーザーの入力を受けて、カメラを回転させるためのインターフェース
    void RollCamera(Vector2 interval);
}

interface IPinchInOut{
    void PinchInOut(float percentage);
}

public class CameraMover : MonoBehaviour, IRollCamera, IPinchInOut
{
    [SerializeField] float initialRadius = 20.0f;
    [SerializeField] float initialTheata = 45.0f; //デフォルト視点におけるX軸周りのカメラの傾き
    [SerializeField] float initialPhi = 45.0f; //デフォルト視点におけるY軸周りのカメラの傾き
    [SerializeField] float fitThetaChange = 0.1f; //カメラのX軸周りの回転速度の補正値
    [SerializeField] float fitPhiChange = 0.1f; //カメラのY軸周りの回転速度の補正値
    [SerializeField] float fitRadiusChange = 1.0f; //ピンチインピンチアウトの変化量の補正値
    
    //視点が反転しないようX軸周りの回転は最大90度で設定する
    private enum ThetaRange{
        Min = 0,
        Max = 90
    }

    //ピンチインピンチアウトの上限下限
    private enum RadiusRange{
        Min = 10,
        Max = 30
    }
    private float tmpTheta;
    private float tmpPhi;
    private float tmpRadius;

    void Start(){
        InitializeCameraPos();
    }


    private void UpdateCamerPos(float radius, float theta, float phi){
        //カメラポジションを更新する
        float posX = radius * Mathf.Cos(theta * Mathf.PI / 180.0f) * Mathf.Sin(phi * Mathf.PI / 180.0f);
        float posY = radius * Mathf.Sin(theta * Mathf.PI / 180.0f);
        float posZ = radius * Mathf.Cos(theta * Mathf.PI / 180.0f) * Mathf.Cos(phi * Mathf.PI / 180.0f);
        this.gameObject.transform.position = new Vector3(posX, posY, posZ);
        this.gameObject.transform.rotation = Quaternion.Euler(theta, 180.0f + phi, 0); //カメラのデフォルトの向きがZ軸正のため、Y軸周りに反転させる
    }

    public void InitializeCameraPos(){
        //カメラポジションの初期化
        tmpTheta = initialTheata;
        tmpPhi = initialPhi;
        tmpRadius = initialRadius;
        UpdateCamerPos(tmpRadius, initialTheata, initialPhi);
    }


    public void RollCamera(Vector2 interval){
        //カメラ視点回転実装
        tmpPhi += interval.x * fitPhiChange;
        tmpTheta += (-interval.y) * fitThetaChange; //上下の移動はスクロール方向と逆にする
        //thetaが規定範囲を超えた場合は、規定値内に補正
        if(tmpTheta < (float)ThetaRange.Min){
            tmpTheta = (float)ThetaRange.Min;
        }
        else if(tmpTheta > (float)ThetaRange.Max){
            tmpTheta = (float)ThetaRange.Max;
        }
        UpdateCamerPos(tmpRadius, tmpTheta, tmpPhi);
    }

    public void PinchInOut(float percentage){
        //カメラのピンチインピンチアウト実装
        tmpRadius *= percentage * fitRadiusChange;
        if(tmpRadius< (float)RadiusRange.Min){
            tmpRadius = (float)RadiusRange.Min;
        }
        else if(tmpRadius > (float)RadiusRange.Max){
            tmpRadius = (float)RadiusRange.Max;
        }
        UpdateCamerPos(tmpRadius, tmpTheta, tmpPhi);
    }
}