using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BreakableBlockCollider : MonoBehaviour
{
    [SerializeField] public UnityEvent onBreakingBlock;

    private void OnTriggerStay2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (otherGameObject.layer != LayerMask.NameToLayer(Utils.LaddersLayerName)) return;
        
        var ladder = Utils.GetPipeRoot(otherGameObject.transform).GetComponent<LadderScript>();
        if (ladder.GetBases().Contains(otherGameObject.name))
        {
            onBreakingBlock.Invoke();
        }
    }
}
