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

    void Start()
    {
        _children = (from Transform child in transform select child.GameObject()).ToArray();
        _breakingCoroutine = null;

        foreach (var colliderHandler in GetComponentsInChildren<BreakableBlockCollider>())
        {
            colliderHandler.onBreakingBlock.AddListener(Break);
            Debug.Log("New listener");
        }
    }

    private void Break()
    {
        _breakingCoroutine ??= StartCoroutine(BreakCoroutine());
    }
    
    private IEnumerator BreakCoroutine()
    {
        Debug.Log("Break coroutine started");
        isBroken = true;
        // StartAnimation
        yield return new WaitForSeconds(2); // Wait for animation to end
        foreach (var child in _children)
            child.SetActive(false);
        _breakingCoroutine = null;
    }

    [ContextMenu("Undo brea")]
    public void UndoBreak()
    {
        isBroken = false;
        if (_breakingCoroutine is not null)
            StopCoroutine(_breakingCoroutine); // Change animation to idle
        foreach (var child in _children)
            child.SetActive(true);
    }
}
