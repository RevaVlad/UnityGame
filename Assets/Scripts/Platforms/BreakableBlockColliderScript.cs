using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BreakableBlockCollider : MonoBehaviour
{
    [SerializeField] public UnityEvent onBreakingBlock;
    [SerializeField] private List<(LadderScript ladder, string colldingTile)> _collidingLadders = new ();

    private void FixedUpdate()
    {
        var anyLadderOnTop = false;
        foreach (var (ladder, collidingTile) in _collidingLadders)
            if (ladder.GetBases().Contains(collidingTile))
            {
                anyLadderOnTop = true;
                break;
            }

        if (anyLadderOnTop)
        {
            _collidingLadders.Clear();
            onBreakingBlock.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (otherGameObject.layer == LayerMask.NameToLayer(Utils.LaddersLayerName))
            _collidingLadders.Add((Utils.GetPipeRoot(otherGameObject.transform).GetComponent<LadderScript>(), otherGameObject.name));
    }

    private void OnTriggerExit(Collider other)
    {
        var otherGameObject = other.gameObject;
        if (otherGameObject.layer == LayerMask.NameToLayer(Utils.LaddersLayerName))
            _collidingLadders =
                _collidingLadders.Where(ladderAndTile => ladderAndTile.colldingTile != other.gameObject.name).ToList();
    }
}
