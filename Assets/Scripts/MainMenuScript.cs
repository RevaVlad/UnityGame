using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MainMenuScript : MonoBehaviour
{
    private InputSystemUIInputModule _uiInput;
    [SerializeField] private GameObject newGameButton;
    private void Start()
    {
        _uiInput = GetComponentInChildren<InputSystemUIInputModule>();
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }
    
    void Update()
    {
        
    }
}
