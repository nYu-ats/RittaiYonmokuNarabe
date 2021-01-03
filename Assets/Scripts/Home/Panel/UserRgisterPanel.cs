using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserRgisterPanel : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text inputText;
    [SerializeField] Text userNameAlreadyExist;
    [SerializeField]  RegisterButton registerButton;

    //InputFieldの表示更新処理
    public void ReflectInputText()
    {
        inputText.text = inputField.text;
        registerButton.SendText = inputField.text;
    }
}
