using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button button;
    public static int storedValue = 0;

    void Start() {
        inputField.text = storedValue.ToString();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick() {
        if (int.TryParse(inputField.text, out int newValue))
        {
            storedValue = newValue;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
