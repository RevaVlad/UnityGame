using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class BreakableBlockScript : MonoBehaviour
{
    private GameObject[] _children;
    private Coroutine _breakingCoroutine;

    void Start()
    {
        _children = (from Transform child in transform select child.GameObject()).ToArray();
        _breakingCoroutine = null;

        foreach (var colliderHandler in GetComponentsInChildren<BreakableBlockCollider>())
            colliderHandler.onBreakingBlock.AddListener(Break);
    }

    private void Break()
    {
        if (_breakingCoroutine is not null)
            _breakingCoroutine = StartCoroutine(BreakCoroutine());   
    }
    
    private IEnumerator BreakCoroutine()
    {
        // StartAnimation
        yield return new WaitForSeconds(2); // Wait for animation to end
        foreach (var child in _children)
            child.SetActive(false);
        _breakingCoroutine = null;
    }

    private void UndoBreak()
    {
        if (_breakingCoroutine is not null)
            StopCoroutine(_breakingCoroutine); // Change animation to idle
        foreach (var child in _children)
            child.SetActive(true);
    }
}
