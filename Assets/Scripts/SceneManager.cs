using UnityEngine;

public class ObjectSnapshot
{
    public GameObject GameObject { get; }
    public Vector3 Position { get; }

    public ObjectSnapshot(GameObject gameObject, Vector3 position)
    {
        GameObject = gameObject;
        Position = position;
    }
}

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
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
        var collider = Physics2D.OverlapCircleAll(player.transform.position,
            0.2f, LayerMask.GetMask("Finish"));
        if (collider.Length == 0)
            return;
        var currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var currentLevelNumber = int.Parse(currentSceneName.Split("Level")[1]);
        if (currentLevelNumber < totalLevelCount)
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{currentLevelNumber + 1}");
    }
}