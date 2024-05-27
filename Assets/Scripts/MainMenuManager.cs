using UnityEngine;

public class MainMenuManager : MonoBehaviour
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

    public void OnSettingsPress()
    {
        MenusController.SwitchMenu(mainMenuCanvas, volumeSettingsCanvas, volumeSettingsFirst);
    }

    public void OnReturnOnMainMenuPress()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");
    }

    public void OnVolumeMenuBackPress()
    {
        MenusController.SwitchMenu(volumeSettingsCanvas, mainMenuCanvas, mainMenuFirst);
    }
}