using UnityEngine;

public class ClosePanelButton : MonoBehaviour
{
    [SerializeField] GameObject thisPanel;
    public void OnClicked(){
        thisPanel.SetActive(false);
    }
}
