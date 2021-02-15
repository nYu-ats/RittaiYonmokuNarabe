using UnityEngine;

public class GoTransparentPanel : MonoBehaviour
{
    [SerializeField] GoTransparentButton[] goTransparentButtons;
    [SerializeField] BoardController boardController;

    //ボードが更新されたら碁の透過状態をいったん解除する
    void Start(){
        boardController.boardUpdated += ResetTransparent;
    }

    private void ResetTransparent(){
        foreach(GoTransparentButton btn in goTransparentButtons){
            btn.ResetTransparent();
        }
    }
}
