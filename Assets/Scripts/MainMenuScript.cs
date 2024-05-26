using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MainMenuScript : MonoBehaviour
{
    private InputSystemUIInputModule uiInput;
    private void Start()
    {
        uiInput = GetComponentInChildren<InputSystemUIInputModule>();
        if (!PlayerPrefs.HasKey("currentLevel"))
            GameObject.Find("ContinueButton").SetActive(false);
    }

    public void OnContinueGame()
    {
        SceneManager.LoadLastLevel();
    }
}
