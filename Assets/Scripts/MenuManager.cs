using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
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

        private InputActionAsset sceneInput;
        private PlayerInput playerInput;
        private SceneManager sceneManager;

        private bool _isPaused;

        private void Start()
        {
            sceneManager = GameObject.Find("SceneManager").transform.GetComponent<SceneManager>();
            sceneInput = sceneManager.PlayerInput.actions;
            playerInput = player.transform.GetComponent<PlayerInput>();
            _mainMenuCanvas.SetActive(false);
            _settingsMenuCanvas.SetActive(false);
            _settingsVolumeCanvas.SetActive(false);
        }

        private void Update()
        {
            if (sceneManager.MenuOpenInput)
            {
                if (!_isPaused)
                    Pause();
            }
            else if (sceneManager.MenuCloseInput)
            {
                if (_isPaused)
                    UnPause();
            }
        }

        #region Pause/Unpause Function

        private void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
            sceneInput.FindActionMap("Control").Disable();
            sceneInput.FindActionMap("UI").Enable();
            playerInput.DeactivateInput();
            OpenMainMenu();
        }

        private void UnPause()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            sceneInput.FindActionMap("UI").Disable();
            sceneInput.FindActionMap("Control").Enable();
            playerInput.ActivateInput();
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
            sceneManager.OnRestartLevel();
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
}