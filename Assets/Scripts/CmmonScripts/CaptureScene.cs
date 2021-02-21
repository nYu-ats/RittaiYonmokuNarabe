using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureScene : MonoBehaviour
{
    private string filePath = "Assets/Image/screenShot";
    private int captureNumber = 2;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            filePath += captureNumber.ToString();
            filePath += ".png";
            ScreenCapture.CaptureScreenshot(filePath);
            captureNumber += 1;
        }
    }
}
