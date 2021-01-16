using UnityEngine;

public class ReturnButton : MonoBehaviour
{
    [SerializeField] HomePanel homePanel;
    
    public void OnButtonClicked(){
        homePanel.isActivePanel.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
