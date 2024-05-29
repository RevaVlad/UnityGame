using UnityEngine;

public class ObjectSnapshot
{
    public GameObject GameObject { get; }
    public Vector3 Position { get; }

    public ObjectSnapshot(GameObject gameObject, Vector3 position)
    {
        GameObject = gameObject;
        Position = position;
    }
}