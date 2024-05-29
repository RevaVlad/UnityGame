using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
    private GameObject player;
    [SerializeField] private int totalLevelCount = 6;
    private GameObject flashingImage;
    [SerializeField] private AudioClip[] rollbackSound;

    private readonly Stack<List<ObjectSnapshot>> sceneSnapshots = new();
    private GameObject blurManager;

    private PlayerInput playerInput;
    private PlayerInput sceneInput;

    private readonly List<AudioSource> pausedAudioSources = new();

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

        var playerPosition = player.transform.position;
        sceneSnapshot.Add(new ObjectSnapshot(player, new Vector3(playerPosition.x, playerPosition.y)));
        sceneSnapshots.Push(sceneSnapshot);
    }

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerInput = player.GetComponent<PlayerInput>();
        sceneInput = GetComponent<PlayerInput>();
        GameObject.Find("loadLevel").GetComponent<Animator>().enabled = true;
        blurManager = GameObject.Find("BlurManager");
        blurManager.SetActive(false);
        flashingImage = gameObject.transform.Find("rollback").gameObject;
        flashingImage.SetActive(false);
    }

    private void Update() => CheckFinishAndLoadNextLevel();

    public void OnRestartLevel() => LoadLastLevel();

    private void OnRestoreSnapshot()
    {
        if (!sceneSnapshots.TryPop(out var snapshot))
            return;
        sceneInput.DeactivateInput();
        playerInput.actions.FindActionMap("LadderInput").Disable();
        playerInput.actions.FindActionMap("BasicInput").Disable();
        StartCoroutine(RestoreSnapshotAnimation(snapshot));
    }

    private IEnumerator RestoreSnapshotAnimation(List<ObjectSnapshot> snapshot)
    {
        flashingImage.SetActive(true);
        var flashCoroutine = StartCoroutine(FlashImage());

        PauseAllSounds();

        SoundFXManager.instance.PlaySoundFXClip(rollbackSound, transform, 1.2f);
        yield return StartCoroutine(BlurEffect(true));

        foreach (var objSnap in snapshot)
            objSnap.GameObject.transform.position = objSnap.Position;

        var playerScript = player.GetComponent<HeroScript>();
        playerScript.OnDropLadder();

        playerInput.actions.FindActionMap("BasicInput").Enable();

        yield return StartCoroutine(BlurEffect(false));

        ResumeAllSounds();
        StopCoroutine(flashCoroutine);
        flashingImage.SetActive(false);
        sceneInput.ActivateInput();
    }

    private void PauseAllSounds()
    {
        var allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in allAudioSources)
        {
            if (!audioSource.isPlaying) continue;
            audioSource.Pause();
            pausedAudioSources.Add(audioSource);
        }
    }

    private void ResumeAllSounds()
    {
        foreach (var audioSource in pausedAudioSources.Where(audioSource => !audioSource.IsDestroyed()))
            audioSource.UnPause();
        pausedAudioSources.Clear();
    }

    private IEnumerator BlurEffect(bool apply)
    {
        if (apply)
            blurManager.SetActive(true);

        var blurManagerVolume = blurManager.GetComponent<Volume>();
        if (!blurManagerVolume.profile.TryGet(out UnityEngine.Rendering.Universal.DepthOfField depthOfField))
            yield break;

        var blurStep = apply ? 7f : -7f;
        for (var i = 0; i < 20; i++)
        {
            depthOfField.focalLength.value += blurStep;
            yield return new WaitForSeconds(0.04f);
        }

        if (!apply)
            blurManager.SetActive(false);
    }

    private IEnumerator FlashImage()
    {
        while (true)
        {
            flashingImage.SetActive(!flashingImage.activeSelf);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public static void SaveLevelNumber(int levelNumber) => PlayerPrefs.SetInt("currentLevel", levelNumber);

    private static int GetCurrentSceneNumber(string sceneName) => int.Parse(sceneName.Split("Level")[1]);

    private static string GetCurrentSceneName() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

    private void CheckFinishAndLoadNextLevel()
    {
        var collider = Physics2D.OverlapCircleAll(player.transform.position, 0.2f, LayerMask.GetMask("Finish"));
        if (collider.Length == 0)
            return;

        var currentLevelNumber = GetCurrentSceneNumber(GetCurrentSceneName());
        if (currentLevelNumber >= totalLevelCount) return;
        SaveLevelNumber(currentLevelNumber + 1);
        LoadLastLevel();
    }

    public static void LoadLastLevel()
    {
        var lastLevel = PlayerPrefs.GetInt("currentLevel", 1);
        SaveLevelNumber(lastLevel);
        UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{lastLevel}");
    }
}