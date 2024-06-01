using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GetNearbyObjectsScript : MonoBehaviour
{
    [SerializeField] private string[] layersToProcess;
    private int[] layersToProcessInt;
    [field: SerializeField] public List<GameObject> CollidingObjects { get; private set; } = new();
    public UnityEvent EnterCollider { get; private set; }
    public UnityEvent ExitCollider { get; private set; }

    private void Awake()
    {
        layersToProcessInt = layersToProcess.Select(LayerMask.NameToLayer).ToArray();
        EnterCollider = new UnityEvent();
        ExitCollider = new UnityEvent();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (!layersToProcessInt.Contains(otherGameObject.layer)) return;
        CollidingObjects.Add(otherGameObject);
        EnterCollider.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (!layersToProcessInt.Contains(otherGameObject.layer)) return;
        CollidingObjects.Remove(otherGameObject);
        ExitCollider.Invoke();
    }
}