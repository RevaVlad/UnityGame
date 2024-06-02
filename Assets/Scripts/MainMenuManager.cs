using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject volumeSettingsCanvas;

    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject volumeSettingsFirst;

    private GameObject mainBackground;
    private GameObject settingsBackground;

    private void Start()
    {
        mainBackground = GameObject.Find("MainMenuBackground");
        settingsBackground = GameObject.Find("MainMenuSettingsBackground");
        settingsBackground.SetActive(false);
        if (!PlayerPrefs.HasKey("currentLevel"))
            GameObject.Find("ContinueButton").SetActive(false);
    }

    public void OnContinueGame() => SceneManager.LoadLastLevel();

    public void OnNewGame()
    {
        SceneManager.SaveLevelNumber(0);
        OnContinueGame();
    }

    public void OnSettingsPress()
    {
        mainBackground.SetActive(false);
        settingsBackground.SetActive(true);
        MenusController.SwitchMenu(mainMenuCanvas, volumeSettingsCanvas, volumeSettingsFirst);
    }

    public void OnReturnOnMainMenuPress() => UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");

    public void OnVolumeMenuBackPress()
    {
        mainBackground.SetActive(true);
        settingsBackground.SetActive(false);
        MenusController.SwitchMenu(volumeSettingsCanvas, mainMenuCanvas, mainMenuFirst);
    }
}