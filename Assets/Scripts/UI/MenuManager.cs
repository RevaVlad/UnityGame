using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        [Header("MenuObjects")]
        [SerializeField] private GameObject _mainMenuCanvas;
        [SerializeField] private GameObject _settingsMenuCanvas;
        [SerializeField] private GameObject _settingsVolumeCanvas;

        [Header("First Selected Option")] 
        [SerializeField] private GameObject _mainMenuFirst;
        [SerializeField] private GameObject _settingsMenuFirst;
        [SerializeField] private GameObject _settingsVolumeFirst;
        
        [Header("Player")]
        [SerializeField] private GameObject player;

        private bool _isPaused = false;
        private void Start()
        {
            _mainMenuCanvas.SetActive(false);
            _settingsMenuCanvas.SetActive(false);
            _settingsVolumeCanvas.SetActive(false);
        }

        private void Update()
        {
            if (SceneManager.Instance.MenuOpenInput)
            {
                if (!_isPaused)
                    Pause();
            }
            else if (SceneManager.Instance.MenuCloseInput)
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
            SceneManager.PlayerInput.actions.FindActionMap("Control").Disable();
            SceneManager.PlayerInput.actions.FindActionMap("UI").Enable();
            player.transform.GetComponent<PlayerInput>().DeactivateInput();
            OpenMainMenu();
        }

        private void UnPause()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            SceneManager.PlayerInput.actions.FindActionMap("UI").Disable();
            SceneManager.PlayerInput.actions.FindActionMap("Control").Enable();
            player.transform.GetComponent<PlayerInput>().ActivateInput();
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
}
