using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


public class MenuManager : MonoBehaviour
{
    [Header("MenuObjects")] [SerializeField]
    private GameObject _mainMenuCanvas;

    [SerializeField] private GameObject _settingsMenuCanvas;
    [SerializeField] private GameObject _settingsVolumeCanvas;

    [Header("First Selected Option")] [SerializeField]
    private GameObject _mainMenuFirst;

    [SerializeField] private GameObject _settingsMenuFirst;
    [SerializeField] private GameObject _settingsVolumeFirst;

    [Header("Player")] [SerializeField] private GameObject player;
    [Header("Scene")] [SerializeField] private GameObject scene;

    private PlayerInput _playerInput;
    private PlayerInput _sceneInput;
    private InputSystemUIInputModule _uiInput;

    private bool _isPaused;
    private InputActionMap previousActionMap;

    private void Start()
    {
        _sceneInput = scene.transform.GetComponent<PlayerInput>();
        _playerInput = player.transform.GetComponent<PlayerInput>();
        _uiInput = GetComponentInChildren<InputSystemUIInputModule>();
        _uiInput.actionsAsset.Disable();

        _mainMenuCanvas.SetActive(false);
        _settingsMenuCanvas.SetActive(false);
        _settingsVolumeCanvas.SetActive(false);
    }


    private void Update()
    {
        if (_sceneInput.currentActionMap.FindAction("MenuOpen").triggered)
        {
            if (!_isPaused)
                Pause();
        }
        else if (_uiInput.actionsAsset.FindActionMap("UI").FindAction("MenuClose").triggered)
        {
            if (_isPaused)
                UnPause();
        }
    }

    #region Pause/Unpause Function

    private void Pause()
    {
        _isPaused = true;
        _sceneInput.DeactivateInput();
        previousActionMap = _playerInput.actions.FindActionMap(_playerInput.actions.FindActionMap("BasicInput").enabled
            ? "BasicInput"
            : "LadderInput");
        _playerInput.actions.FindActionMap("LadderInput").Disable();
        _playerInput.actions.FindActionMap("BasicInput").Disable();
        _uiInput.actionsAsset.Enable();
        OpenMainMenu();
    }

    private void UnPause()
    {
        _isPaused = false;
        _sceneInput.ActivateInput();
        if (previousActionMap == _playerInput.actions.FindActionMap("BasicInput"))
        {
            _playerInput.actions.FindActionMap("LadderInput").Disable();
            _playerInput.actions.FindActionMap("BasicInput").Enable();
        }
        else
        {
            _playerInput.actions.FindActionMap("BasicInput").Disable();
            _playerInput.actions.FindActionMap("LadderInput").Enable();
        }
        
        _uiInput.actionsAsset.Disable();
        CloseAllMenus();
    }

    #endregion

    #region OpenMenus/CloseMenus

    private void OpenMainMenu()
    {
        _mainMenuCanvas.SetActive(true);
        _settingsMenuCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void CloseAllMenus()
    {
        _mainMenuCanvas.SetActive(false);
        _settingsMenuCanvas.SetActive(false);
        _settingsVolumeCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OpenSettingsMenu()
    {
        _settingsMenuCanvas.SetActive(true);
        _mainMenuCanvas.SetActive(false);
        _settingsVolumeCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }

    private void OpenVolumeSettingsMenu()
    {
        _settingsMenuCanvas.SetActive(false);
        _settingsVolumeCanvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(_settingsVolumeFirst);
    }

    #endregion

    #region Main Menu Button Actions

    public void OnSettingsPress()
    {
        OpenSettingsMenu();
    }

    public void OnResumePress()
    {
        UnPause();
    }

    public void OnRestartPress()
    {
        UnPause();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            .buildIndex);
    }

    #endregion

    #region Settings Menu Button Actions

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }

    public void OnSettingsGoToVolumePress()
    {
        OpenVolumeSettingsMenu();
    }

    #endregion

    #region Volume Menu Button Actions

    public void OnVolumeMenuBackPress()
    {
        OpenSettingsMenu();
    }

    #endregion
}