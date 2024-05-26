using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject volumeSettingsCanvas;

    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject volumeSettingsFirst;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("currentLevel"))
            GameObject.Find("ContinueButton").SetActive(false);
    }

    public void OnContinueGame()
    {
        SceneManager.LoadLastLevel();
    }

    public void OnNewGame()
    {
        SceneManager.SaveLevelNumber(1);
        OnContinueGame();
    }


    private void OpenMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        volumeSettingsCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    private void OpenVolumeSettingsMenu()
    {
        volumeSettingsCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(volumeSettingsFirst);
    }


    public void OnSettingsPress()
    {
        OpenVolumeSettingsMenu();
    }

    public void OnExitPress()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");
    }


    public void OnVolumeMenuBackPress()
    {
        OpenMainMenu();
    }
}