using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoTransparentButton : MonoBehaviour
{
    [SerializeField] Material goMat;
    private bool isTransparented = false;
    private (Color transparent, Color normal) buttnColor = (transparent: new Color(0.5f, 0.5f, 0.5f ,1), normal: new Color(1, 1, 1, 1));
    private (float transparent, float normal) colorA = (transparent: 0.1f, normal:1.0f);

    void Start(){
        goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.normal);
    }

    public void OnClicked(){
        if(isTransparented){
            goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.normal);
            this.GetComponent<Image>().color = buttnColor.normal;
            isTransparented = false;
        }
        else{
            goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.transparent);
            this.GetComponent<Image>().color = buttnColor.transparent;
            isTransparented = true;  
        }
    }

    public void ResetTransparent(){
        goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.normal);
        this.GetComponent<Image>().color = buttnColor.normal;
        isTransparented = false;
    }
}
