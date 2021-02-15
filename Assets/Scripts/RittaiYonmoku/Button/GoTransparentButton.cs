using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

interface IResetTransparent{
    void ResetTransparent();
}

public class GoTransparentButton : MonoBehaviour, IResetTransparent
{
    //碁のマテリアルを直接操作して透過処理を行う
    [SerializeField] Material goMat;
    [SerializeField] Image transparentedIcon;
    [SerializeField] Image normalIcon;
    [SerializeField] PlaySE playSE;
    private bool isTransparented = false;
    private (float transparent, float normal) colorA = (transparent: 0.1f, normal:1.0f);


    void Start(){
        goMat.color = new Color(goMat.color.r, goMat.color.g, goMat.color.b, colorA.normal);
        transparentedIcon.enabled = false;
        normalIcon.enabled = true;
    }

    public void OnClicked(){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        //現在透過状態かそうでないかによって処理を切り分ける
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
