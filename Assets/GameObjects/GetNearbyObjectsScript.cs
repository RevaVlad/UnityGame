using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GetNearbyObjectsScript : MonoBehaviour
{
    [SerializeField] private string[] layersToProcess;
    private int[] layersToProcessInt;
    [SerializeField] internal List<GameObject> collidingObjects = new ();

    private void Start()
    {
        layersToProcessInt = layersToProcess.Select(layer => LayerMask.NameToLayer(layer)).ToArray();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var gameObject = other.gameObject;
        if (layersToProcessInt.Contains(gameObject.layer))
        {
            collidingObjects.Add(gameObject);
            Debug.Log($"Added object: {gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var gameObject = other.gameObject;
        if (layersToProcessInt.Contains(gameObject.layer))
            collidingObjects.Remove(gameObject);
    }
}
