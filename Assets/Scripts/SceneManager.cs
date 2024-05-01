using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private int totalLevelCount = 6;

    private void Update()
    {
        CheckFinishAndLoadNextLevel();
    }

    private void OnRestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            .buildIndex);
    }

    private void CheckFinishAndLoadNextLevel()
    {
        var collider = Physics2D.OverlapCircleAll(transform.position,
            0.2f, LayerMask.GetMask("Finish"));
        if (collider.Length == 0)
            return;
        var currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var currentLevelNumber = int.Parse(currentSceneName.Split("Level")[1]);
        if (currentLevelNumber < totalLevelCount)
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{currentLevelNumber + 1}");
    }
}