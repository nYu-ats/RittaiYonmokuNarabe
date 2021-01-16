using UnityEngine;
using CommonConfig;

public class ReviewButton : MonoBehaviour
{
    public void OnButtonClicked(){
        Application.OpenURL(URL.GoogleAppStoreURL + URL.ThisAppId);
    }
}
