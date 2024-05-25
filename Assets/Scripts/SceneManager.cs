using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

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
    private SceneTransition animatorManager;

    private Animator restartLevelAnimator;
    private Animator nextLevelAnimator;


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
        sceneSnapshot.Add(new ObjectSnapshot(player, new Vector3(playerPosition.x, playerPosition.y)));
        _sceneSnapshots.Push(sceneSnapshot);
    }

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        restartLevelAnimator = GameObject.Find("Panel").GetComponent<Animator>();
        nextLevelAnimator = GameObject.Find("Image").GetComponent<Animator>();
        restartLevelAnimator.enabled = true;
    }


    private void Update()
    {
        CheckFinishAndLoadNextLevel();
    }

    public void OnRestartLevel()
    {
        LoadLastLevel();
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

    private static void SaveCurrentLevelNumber(int levelNumber)
    {
        PlayerPrefs.SetInt("currentLevel", levelNumber);
    }

    private static int GetCurrentSceneNumber(string sceneName)
    {
        return int.Parse(sceneName.Split("Level")[1]);
    }

    private static string GetCurrentSceneName()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    private void CheckFinishAndLoadNextLevel()
    {
        var collider = Physics2D.OverlapCircleAll(player.transform.position, 0.2f, LayerMask.GetMask("Finish"));
        if (collider.Length == 0)
            return;

        var currentLevelNumber = GetCurrentSceneNumber(GetCurrentSceneName());
        if (currentLevelNumber >= totalLevelCount) return;
        SaveCurrentLevelNumber(currentLevelNumber + 1);
        UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{currentLevelNumber + 1}");
        //StartCoroutine(SceneTransition.StartAnimation(animatorManager.NextLevelAnimator));
    }

    private static void LoadLastLevel()
    {
        var lastLevel = PlayerPrefs.GetInt("currentLevel", 1);
        SaveCurrentLevelNumber(lastLevel);
        UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{lastLevel}");
    }
}