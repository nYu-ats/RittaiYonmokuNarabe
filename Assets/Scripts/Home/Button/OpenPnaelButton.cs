using UnityEngine;

public class OpenPnaelButton : MonoBehaviour
{
    [SerializeField] GameObject openPanel;
    //戻るボタンからActive状態のパネルを操作する必要があるため、その情報をHomePanelに持たせている
    //HomaPanelのActivePanelを更新するためイベント
    public delegate void PanelActivateEvent(GameObject panel);
    public event PanelActivateEvent panelActiveEvent = (GameObject panel) => {};
    public void OnButtonClicked(){
        openPanel.SetActive(true);
        panelActiveEvent(openPanel);
    }

}

