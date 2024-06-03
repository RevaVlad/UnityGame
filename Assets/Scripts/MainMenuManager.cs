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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        DestroyAllDontDestroyOnLoadObjects();
        mainBackground = GameObject.Find("MainMenuBackground");
        settingsBackground = GameObject.Find("MainMenuSettingsBackground");
        settingsBackground.SetActive(false);
        if (!PlayerPrefs.HasKey("currentLevel"))
            GameObject.Find("ContinueButton").SetActive(false);
    }

    private static void DestroyAllDontDestroyOnLoadObjects()
    {
        var connect = new GameObject();
        DontDestroyOnLoad(connect);

        foreach (var root in connect.scene.GetRootGameObjects())
            Destroy(root);
    }

    public void OnContinueGame() => SceneManager.LoadLastLevel();

    public void OnNewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartCutscene");
    }

    public void OnSettingsPress()
    {
        mainBackground.SetActive(false);
        settingsBackground.SetActive(true);
        MenusController.SwitchMenu(mainMenuCanvas, volumeSettingsCanvas, volumeSettingsFirst);
    }

    public void OnExitPress() => Application.Quit();

    public void OnVolumeMenuBackPress()
    {
        mainBackground.SetActive(true);
        settingsBackground.SetActive(false);
        MenusController.SwitchMenu(volumeSettingsCanvas, mainMenuCanvas, mainMenuFirst);
    }
}