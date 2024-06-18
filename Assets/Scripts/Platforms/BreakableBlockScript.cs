using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class BreakableBlockScript : MonoBehaviour
{
    public bool IsBroken { get; private set; }
    [SerializeField] private GameObject[] _children;
    private Coroutine _breakingCoroutine;
    [SerializeField] private AudioClip[] stoneCrashSound;

    private static readonly int isBrokenAnim = Animator.StringToHash("isBrokenAnim");

    private void Awake() =>
        transform.SetParent(GameObject.Find("BreakBlockContainer") is null
            ? new GameObject("BreakBlockContainer").transform
            : GameObject.Find("BreakBlockContainer").transform);

    private void Start()
    {
        _children = (from Transform child in transform select child.GameObject()).ToArray();
        _breakingCoroutine = null;

        foreach (var colliderHandler in GetComponentsInChildren<BreakableBlockCollider>())
        {
            colliderHandler.onBreakingBlock.AddListener(Break);
        }
    }

    private void Break()
    {
        _breakingCoroutine ??= StartCoroutine(BreakCoroutine());
    }

    private IEnumerator BreakCoroutine()
    {
        IsBroken = true;
        foreach (var animator in GetComponentsInChildren<Animator>())
        {
            animator.SetBool(isBrokenAnim, true);
        }

        SoundFXManager.Instance.PlaySoundFXClip(stoneCrashSound, transform, 1.2f);
        yield return new WaitForSeconds(1);
        foreach (var child in _children)
            child.SetActive(false);
        _breakingCoroutine = null;
    }

    [ContextMenu("Undo break")]
    public void UndoBreak()
    {
        IsBroken = false;
        if (_breakingCoroutine is not null)
            StopCoroutine(_breakingCoroutine);
        foreach (var child in _children)
            child.SetActive(true);

        foreach (var animator in GetComponentsInChildren<Animator>())
        {
            animator.SetBool(isBrokenAnim, false);
        }
    }
}