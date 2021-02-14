using UnityEngine;
using CommonConfig;

public class ReturnButton : MonoBehaviour
{
    [SerializeField] HomePanel homePanel;
    [SerializeField] PlaySE playSE;    
    public void OnButtonClicked(){
        playSE.PlaySound(AudioConfig.ReturnIndex);
        homePanel.isActivePanel.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
