using System.Collections.Generic;
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

    private readonly Stack<List<ObjectSnapshot>> sceneSnapshots = new();

    public void CreateObjectsSnapshot()
    {
        if (sceneSnapshots.Count == 100) sceneSnapshots.Clear();
        var sceneSnapshot = new List<ObjectSnapshot>();
        var laddersContainer = GameObject.Find("LaddersContainer").transform;
        for (var i = 0; i < laddersContainer.childCount; i++)
        {
            var obj = laddersContainer.GetChild(i).gameObject;
            var position = obj.transform.position;
            sceneSnapshot.Add(new ObjectSnapshot(obj, new Vector3(position.x, position.y)));
        }

        var playerObj = GameObject.Find("Player");
        var playerPosition = playerObj.transform.position;
        sceneSnapshot.Add(new ObjectSnapshot(playerObj,
            new Vector3(playerPosition.x, playerPosition.y)));
        sceneSnapshots.Push(sceneSnapshot);
    }

    private void RestorePreviousSnapshot()
    {
        if (!sceneSnapshots.TryPop(out var snapshot))
            return;
        foreach (var objSnap in snapshot)
            objSnap.GameObject.transform.position = objSnap.Position;
    }

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