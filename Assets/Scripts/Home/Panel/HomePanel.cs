using UnityEngine;
using UnityEngine.UI;

public class HomePanel : MonoBehaviour
{
    [SerializeField] GameObject returnButton;
    [SerializeField] Button volumeSettingOpenButton;
    public GameObject isActivePanel;

    void Start(){
        //イベントハンドラーを追加
        volumeSettingOpenButton.GetComponent<OpenPnaelButton>().panelActiveEvent += SwitchActivePanel;
    }

    //パネルを開くボタンからのイベントを受け取って
    //IsActivePanelの更新と戻るボタンのActivateを行う
    private void SwitchActivePanel(GameObject panel){
        isActivePanel = panel;
        returnButton.SetActive(true);
    }
}
