using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class BreakableBlockScript : MonoBehaviour
{
    [SerializeField] public bool isBroken;
    [SerializeField] private GameObject[] _children;
    private Coroutine _breakingCoroutine;
    
    private static readonly int isBrokenAnim = Animator.StringToHash("isBrokenAnim");

    void Start()
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
        isBroken = true;
        foreach (var animator in GetComponentsInChildren<Animator>())
        {
            animator.SetBool(isBrokenAnim, true);
        }
        yield return new WaitForSeconds(.7f);
        foreach (var child in _children)
            child.SetActive(false);
        _breakingCoroutine = null;
    }

    [ContextMenu("Undo break")]
    public void UndoBreak()
    {
        isBroken = false;
        if (_breakingCoroutine is not null)
            StopCoroutine(_breakingCoroutine);
        foreach (var animator in GetComponentsInChildren<Animator>())
        {
            animator.SetBool(isBrokenAnim, false);
        }
        foreach (var child in _children)
            child.SetActive(true);
    }
}
