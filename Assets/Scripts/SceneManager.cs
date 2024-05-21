using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private readonly Stack<List<ObjectSnapshot>> _sceneSnapshots = new();
    public PlayerInput PlayerInput { get; private set; }
    public bool MenuOpenInput { get; private set; }
    public bool MenuCloseInput { get; private set; }

    private InputAction menuOpenAction;
    private InputAction menuCloseAction;

    public void CreateObjectsSnapshot()
    {
        if (_sceneSnapshots.Count == 100) _sceneSnapshots.Clear();
        var sceneSnapshot = new List<ObjectSnapshot>();
        var laddersContainer = GameObject.Find("LaddersContainer").transform;
        for (var i = 0; i < laddersContainer.childCount; i++)
        {
            var obj = laddersContainer.GetChild(i).gameObject;
            var position = obj.transform.position;
            sceneSnapshot.Add(new ObjectSnapshot(obj, new Vector3(position.x, position.y)));
        }

        var playerPosition = player.transform.position;
        sceneSnapshot.Add(new ObjectSnapshot(player,
            new Vector3(playerPosition.x, playerPosition.y)));
        _sceneSnapshots.Push(sceneSnapshot);
    }

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        menuOpenAction = PlayerInput.actions["MenuOpen"];
        menuCloseAction = PlayerInput.actions["MenuClose"];
    }

    private void Update()
    {
        CheckFinishAndLoadNextLevel();
        MenuOpenInput = menuOpenAction.WasPressedThisFrame();
        MenuCloseInput = menuCloseAction.WasPressedThisFrame();
    }

    public void OnRestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            .buildIndex);
    }

    private void OnRestoreSnapshot()
    {
        if (!_sceneSnapshots.TryPop(out var snapshot))
            return;
        foreach (var objSnap in snapshot)
            objSnap.GameObject.transform.position = objSnap.Position;
        var playerScript = player.GetComponent<HeroScript>();
        playerScript.OnDropLadder();
    }

    public static void SaveCurrentLevelNumber()
    {
        PlayerPrefs.SetInt("currentLevel", GetCurrentSceneNumber(GetCurrentSceneName()));
    }

    private static int GetCurrentSceneNumber(string sceneName) => int.Parse(sceneName.Split("Level")[1]);

    private static string GetCurrentSceneName() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

    private void CheckFinishAndLoadNextLevel()
    {
        var collider = Physics2D.OverlapCircleAll(player.transform.position,
            0.2f, LayerMask.GetMask("Finish"));
        if (collider.Length == 0)
            return;
        var currentLevelNumber = GetCurrentSceneNumber(GetCurrentSceneName());
        if (currentLevelNumber < totalLevelCount)
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{currentLevelNumber + 1}");
    }
}