using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerScript : MonoBehaviour
{
    [SerializeField] private bool destroyOnTriggerEnter;
    [SerializeField] private string layerFilter;
    [SerializeField] private UnityEvent onTriggerEnterEvent;
    [SerializeField] private UnityEvent onTriggerExitEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(layerFilter) && other.gameObject.layer != LayerMask.NameToLayer(layerFilter))
            return;
        onTriggerEnterEvent.Invoke();
        if (destroyOnTriggerEnter)
            Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(layerFilter) && other.gameObject.layer != LayerMask.NameToLayer(layerFilter))
            return;
        onTriggerExitEvent.Invoke();
    }
}
