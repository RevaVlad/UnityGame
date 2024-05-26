using UnityEngine;
using UnityEngine.InputSystem.UI;

public class MainMenuScript : MonoBehaviour
{
    private InputSystemUIInputModule _uiInput;
    private void Start()
    {
        _uiInput = GetComponentInChildren<InputSystemUIInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
