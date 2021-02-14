using UnityEngine;
using CommonConfig;

public class OpenURLButton : MonoBehaviour
{
    [SerializeField] PlaySE playSE;
    public void OnButtonClicked(string url){
        playSE.PlaySound(AudioConfig.ButtonPushIndex);
        Application.OpenURL(url);
    }
}
