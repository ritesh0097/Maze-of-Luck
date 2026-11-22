
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasswordInput : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    [SerializeField] GameObject inputarea;
    [SerializeField] GameObject unlockButton;

    [SerializeField] TextMeshProUGUI textOutput;

    [SerializeField] int password;

    public bool isDoorUnlock = true;

    void Start()
    {
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;

        EventSystem.current.SetSelectedGameObject(inputarea);
        inputField.ActivateInputField();
    }

    void Update()
    {
        if (inputField.text.Length >= 6)
        {
            EventSystem.current.SetSelectedGameObject(unlockButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(inputarea);
        }
    }

    public void PasswordSubmit()
    {       

        if (int.TryParse(inputField.text, out int inputPasswordText))
        {
            if (inputPasswordText == password)
            {
                isDoorUnlock = false;                
            }
            else
            {
                textOutput.text = "Password is Incorrect";
                inputField.text = "";                
            }
        }
        else
        {
            textOutput.text = "Please enter a valid password";            
        }
    }
}
