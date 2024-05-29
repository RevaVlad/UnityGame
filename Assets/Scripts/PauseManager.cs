using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject settingsMenuCanvas;
    [SerializeField] private GameObject settingsVolumeCanvas;

    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject settingsMenuFirst;
    [SerializeField] private GameObject settingsVolumeFirst;

    private PlayerInput playerInput;
    private PlayerInput sceneInput;
    private InputSystemUIInputModule uiInput;

    private bool isPaused;
    private InputActionMap previousActionMap;

    private void Start()
    {
        sceneInput = GameObject.Find("SceneManager").transform.GetComponent<PlayerInput>();
        playerInput = GameObject.Find("Player").transform.GetComponent<PlayerInput>();
        uiInput = GetComponentInChildren<InputSystemUIInputModule>();
        uiInput.actionsAsset.Disable();

        MenusController.CloseAllMenus(mainMenuCanvas, settingsMenuCanvas, settingsVolumeCanvas);
    }

    private void Update()
    {
        if (sceneInput.currentActionMap.FindAction("MenuOpen").triggered && !isPaused)
            Pause();
        else if (uiInput.actionsAsset.FindActionMap("UI").FindAction("MenuClose").triggered && isPaused)
            UnPause();
    }

    private void Pause()
    {
        isPaused = true;
        sceneInput.DeactivateInput();
        previousActionMap = playerInput.actions.FindActionMap(playerInput.actions.FindActionMap("BasicInput").enabled
            ? "BasicInput"
            : "LadderInput");
        playerInput.actions.FindActionMap("LadderInput").Disable();
        playerInput.actions.FindActionMap("BasicInput").Disable();
        uiInput.actionsAsset.Enable();
        MenusController.OpenMenu(mainMenuCanvas, mainMenuFirst);
    }

    private void UnPause()
    {
        isPaused = false;
        sceneInput.ActivateInput();
        if (previousActionMap == playerInput.actions.FindActionMap("BasicInput"))
        {
            playerInput.actions.FindActionMap("LadderInput").Disable();
            playerInput.actions.FindActionMap("BasicInput").Enable();
        }
        else
        {
            playerInput.actions.FindActionMap("BasicInput").Disable();
            playerInput.actions.FindActionMap("LadderInput").Enable();
        }

        uiInput.actionsAsset.Disable();
        MenusController.CloseAllMenus(mainMenuCanvas, settingsMenuCanvas, settingsVolumeCanvas);
    }

    public void OnSettingsPress() =>
        MenusController.SwitchMenu(mainMenuCanvas, settingsMenuCanvas, settingsMenuFirst);

    public void OnResumePress() => UnPause();

    public void OnRestartPress()
    {
        UnPause();
        SceneManager.LoadLastLevel();
    }

    public void OnExitPress() => UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");

    public void OnSettingsBackPress() => MenusController.SwitchMenu(settingsMenuCanvas, mainMenuCanvas, mainMenuFirst);

    public void OnSettingsGoToVolumePress() =>
        MenusController.SwitchMenu(settingsMenuCanvas, settingsVolumeCanvas, settingsVolumeFirst);

    public void OnVolumeMenuBackPress() =>
        MenusController.SwitchMenu(settingsVolumeCanvas, settingsMenuCanvas, settingsMenuFirst);
}