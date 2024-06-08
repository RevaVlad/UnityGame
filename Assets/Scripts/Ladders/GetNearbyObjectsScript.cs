using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GetNearbyObjectsScript : MonoBehaviour
{
    [SerializeField] private string[] layersToProcess;
    private int[] layersToProcessInt;
    [field: SerializeField] public List<GameObject> CollidingObjects { get; private set; } = new();
    public UnityEvent<string> EnterCollider { get;} = new();
    public UnityEvent<string> ExitCollider { get;} = new();

    private void Awake()
    {
        layersToProcessInt = layersToProcess.Select(LayerMask.NameToLayer).ToArray();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (!layersToProcessInt.Contains(otherGameObject.layer)) return;
        CollidingObjects.Add(otherGameObject);
        EnterCollider.Invoke(LayerMask.LayerToName(otherGameObject.layer));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherGameObject = other.gameObject;
        if (!layersToProcessInt.Contains(otherGameObject.layer)) return;
        CollidingObjects.Remove(otherGameObject);
        ExitCollider.Invoke(LayerMask.LayerToName(otherGameObject.layer));
    }
}