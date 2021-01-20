using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountPanel : MonoBehaviour
{
    [SerializeField] Text timeCountText;
    private float time;
    private float timeLimit = 90.0f;

    void Update()
    {
        time = timeLimit - Time.time;
        timeCountText.text = "Time : " + (Mathf.Floor(time * 10) / 10).ToString(); //小数第二位まで表示
    }
}
