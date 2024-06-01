using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GetNearbyObjectsScript : MonoBehaviour
{
    [SerializeField] private string[] layersToProcess;
    private int[] layersToProcessInt;
    [field: SerializeField] public List<GameObject> CollidingObjects { get; private set; } = new();
    public UnityEvent AnyCollisionStarted { get; private set; }
    public UnityEvent NoCollisions { get; private set; }

    private void Awake()
    {
        layersToProcessInt = layersToProcess.Select(LayerMask.NameToLayer).ToArray();
        AnyCollisionStarted = new UnityEvent();
        NoCollisions = new UnityEvent();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (!layersToProcessInt.Contains(otherGameObject.layer)) return;
        CollidingObjects.Add(otherGameObject);
        if (CollidingObjects.Count == 1)
            AnyCollisionStarted.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (!layersToProcessInt.Contains(otherGameObject.layer)) return;
        CollidingObjects.Remove(otherGameObject);
        if (CollidingObjects.Count == 0)
            NoCollisions.Invoke();
    }
}