using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    private void GoToMenu()
    {
        PlayerPrefs.DeleteKey("currentLevel");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}
