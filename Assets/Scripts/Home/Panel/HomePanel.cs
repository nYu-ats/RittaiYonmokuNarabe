using UnityEngine;
using UnityEngine.UI;

public class HomePanel : MonoBehaviour
{
    [SerializeField] GameObject returnButton;
    [SerializeField] Button[] PanleOpenButton;
    public GameObject isActivePanel;

    void Start(){
        //イベントハンドラーを追加
        foreach(Button btn in PanleOpenButton){
            btn.GetComponent<OpenPnaelButton>().panelActiveEvent += SwitchActivePanel;
        }
    }

    //パネルを開くボタンからのイベントを受け取って
    //IsActivePanelの更新と戻るボタンのActivateを行う
    private void SwitchActivePanel(GameObject panel){
        isActivePanel = panel;
        returnButton.SetActive(true);
    }
}
