using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GetNearbyObjectsScript : MonoBehaviour
{
    [SerializeField] private string[] layersToProcess;
    private int[] layersToProcessInt;
    [field: SerializeField] public List<GameObject> CollidingObjects { get; private set; } = new();
    public UnityEvent<int> AddedObject { get; private set; }
    public UnityEvent<int> DeletedObject { get; private set; }

    private void Awake()
    {
        layersToProcessInt = layersToProcess.Select(LayerMask.NameToLayer).ToArray();
        AddedObject = new UnityEvent<int>();
        DeletedObject = new UnityEvent<int>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (!layersToProcessInt.Contains(otherGameObject.layer)) return;
        CollidingObjects.Add(otherGameObject);
        AddedObject.Invoke(CollidingObjects.Count);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (!layersToProcessInt.Contains(otherGameObject.layer)) return;
        CollidingObjects.Remove(otherGameObject);
        DeletedObject.Invoke(CollidingObjects.Count);
    }
}