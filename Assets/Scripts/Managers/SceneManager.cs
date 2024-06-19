using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


public class SceneManager : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private int totalLevelCount = 8;
    private GameObject flashingImage;
    [SerializeField] private AudioClip[] rollbackSound;

    private readonly Stack<List<ObjectSnapshot>> sceneSnapshots = new();
    private GameObject blurManager;
    private GameObject rollbackTrigger;

    private PlayerInput playerInput;
    private PlayerInput sceneInput;

    private Transform laddersContainer;
    private Transform breakBlockContainer;
    private readonly List<AudioSource> pausedAudioSources = new();

    public void CreateObjectsSnapshot()
    {
        if (sceneSnapshots.Count == 100) sceneSnapshots.Clear();
        var sceneSnapshot = new List<ObjectSnapshot>();
        CreateLaddersSnapshot(sceneSnapshot);
        CreateBreakBlockSnapshot(sceneSnapshot);
        var playerPosition = player.transform.position;
        sceneSnapshot.Add(new ObjectSnapshot(player, new Vector3(playerPosition.x, playerPosition.y)));
        sceneSnapshots.Push(sceneSnapshot);
    }

    private void CreateLaddersSnapshot(ICollection<ObjectSnapshot> sceneSnapshot)
    {
        for (var i = 0; i < laddersContainer.childCount; i++)
        {
            var obj = laddersContainer.GetChild(i).gameObject;
            var position = obj.transform.position;
            sceneSnapshot.Add(new ObjectSnapshot(obj, new Vector3(position.x, position.y)));
        }
    }

    private void CreateBreakBlockSnapshot(ICollection<ObjectSnapshot> sceneSnapshot)
    {
        if (breakBlockContainer == null) return;
        for (var i = 0; i < breakBlockContainer.childCount; i++)
        {
            var obj = breakBlockContainer.GetChild(i).gameObject;
            sceneSnapshot.Add(new ObjectSnapshot(obj, obj.GetComponent<BreakableBlockScript>().IsBroken));
        }
    }

    public void ClearSceneSnapshots() => sceneSnapshots.Clear();

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
        rollbackTrigger = GameObject.Find("ActivateRollback");
    }

    private void Start()
    {
        laddersContainer = GameObject.Find("LaddersContainer").transform;
        breakBlockContainer = GameObject.Find("BreakBlockContainer").transform;
    }

    private void Update() => CheckFinishAndLoadNextLevel();

    public void OnRestartLevel() => LoadLastLevel();

    private void StopPipesCoroutine()
    {
        player.GetComponent<HeroScript>().timePassedSinceMoveLadder += .3f;
        for (var i = 0; i < laddersContainer.childCount; i++)
            laddersContainer.GetChild(i).GetComponent<LadderScript>().StopMoveCoroutine();
    }

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
        StopPipesCoroutine();
        PauseAllSounds();

        SoundFXManager.Instance.PlaySoundFXClip(rollbackSound, transform, 1.2f);
        yield return StartCoroutine(BlurEffect(true));

        foreach (var objSnap in snapshot)
            if (objSnap.GameObject.name.StartsWith("BreakableBlock") && !objSnap.IsBroken)
                objSnap.GameObject.GetComponent<BreakableBlockScript>().UndoBreak();
            else if (!objSnap.GameObject.name.StartsWith("BreakableBlock"))
                objSnap.GameObject.transform.position = objSnap.Position;

        player.GetComponent<HeroScript>().OnDropLadder();
        playerInput.actions.FindActionMap("BasicInput").Enable();

        yield return StartCoroutine(BlurEffect(false));
        FindObjectsOfType<AudioSource>()[0].Pause();
        StopCoroutine(flashCoroutine);
        flashingImage.SetActive(false);
        ResumeAllSounds();
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
            yield return new WaitForSeconds(0.01f);
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