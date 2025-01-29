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
        //Assigning the function that will execute when the button is clicked.
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick() {
        //Assigning the value from the input field to a static value.
        if (int.TryParse(inputField.text, out int newValue)) {
            storedValue = newValue;
        }
        //Rebuilding the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
