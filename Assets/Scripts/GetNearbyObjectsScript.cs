using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GetNearbyObjectsScript : MonoBehaviour
{
    [SerializeField] private string[] layersToProcess;
    private int[] layersToProcessInt;
    [SerializeField] internal List<GameObject> collidingObjects = new ();
    internal UnityEvent<int> AddedObject;
    internal UnityEvent<int> DeletedObject;

    private void Awake()
    {
        layersToProcessInt = layersToProcess.Select(layer => LayerMask.NameToLayer(layer)).ToArray();
        AddedObject = new UnityEvent<int>();
        DeletedObject = new UnityEvent<int>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var gameObject = other.gameObject;
        if (layersToProcessInt.Contains(gameObject.layer))
        {
            collidingObjects.Add(gameObject);
            AddedObject.Invoke(collidingObjects.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var gameObject = other.gameObject;
        if (layersToProcessInt.Contains(gameObject.layer))
        {
            collidingObjects.Remove(gameObject);
            DeletedObject.Invoke(collidingObjects.Count);
        }
    }
}
