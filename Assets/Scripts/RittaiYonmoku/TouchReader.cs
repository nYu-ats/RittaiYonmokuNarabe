using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchReader : MonoBehaviour
{
    [SerializeField] CameraMover cameraMover;
    private Vector2 baseTapPos;
    private float baseDist;
    void Update()
    {
        if(Input.touchCount == 2){
            CalcPinchPercentage();
        }
        else if(Input.touchCount ==1){
            CalcScrollDistance();
        }
        else{
            return;
        }

    }

    //ピンチインピンチアウトの割合計算
    private void CalcPinchPercentage(){
        Touch touchPoint1 = Input.GetTouch(0);
        Touch touchPoint2 = Input.GetTouch(1);
        if(touchPoint1.phase == TouchPhase.Began & touchPoint2.phase == TouchPhase.Began){
            baseDist = Vector2.Distance(touchPoint1.position, touchPoint2.position);
        }
        else if(touchPoint1.phase == TouchPhase.Moved & touchPoint2.phase == TouchPhase.Moved){
            float tmpDist = Vector2.Distance(touchPoint1.position, touchPoint2.position);
            float changePercentage = tmpDist / baseDist;
            cameraMover.PinchInOut(changePercentage);
            baseDist = tmpDist;
        }
    }

    //画面スクロールの距離計算
    private void CalcScrollDistance(){
        Touch touchPoint = Input.GetTouch(0);
        if(touchPoint.phase == TouchPhase.Began){
        //タップの基準位置取得
        baseTapPos = touchPoint.position;
        }
        else if(touchPoint.phase == TouchPhase.Moved){
            Vector2 tmpTapPos = touchPoint.position;
            Vector2 moveInterval = tmpTapPos - baseTapPos;
            cameraMover.RollCamera(moveInterval);
            baseTapPos = tmpTapPos; //タップの基準位置を更新
        }
    }
}
