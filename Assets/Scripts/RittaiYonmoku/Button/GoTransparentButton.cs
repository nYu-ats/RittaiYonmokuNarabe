using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoTransparentButton : MonoBehaviour
{
    [SerializeField] Material goMat;
    private bool isTransparented = false;
    private (float transparent, float normal) colorA = (transparent: 0.1f, normal:1.0f);
    [SerializeField] Image transparentedIcon;
    [SerializeField] Image normalIcon;

    void Start(){
        goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.normal);
        transparentedIcon.enabled = false;
        normalIcon.enabled = true;
    }

    public void OnClicked(){
        if(isTransparented){
            goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.normal);
            transparentedIcon.enabled = false;
            normalIcon.enabled = true;
            isTransparented = false;
        }
        else{
            goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.transparent);
            transparentedIcon.enabled = true;
            normalIcon.enabled = false;
            isTransparented = true;  
        }
    }

    public void ResetTransparent(){
        goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.normal);
        isTransparented = false;
    }
}
