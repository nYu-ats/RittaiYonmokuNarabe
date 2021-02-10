using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureScene : MonoBehaviour
{
    private float time;
    private float shotTime = 5.0f;
    void Start(){
        time = 0.0f;
    }

    void Update(){
        if(time < shotTime){
            time += Time.deltaTime;
        }
        else{
            ScreenCapture.CaptureScreenshot("Assets/Image/HomeImage2.png");
            Debug.Log("captured");
        }
    }
}