using UnityEngine;

public class ObjectSnapshot
{
    public GameObject GameObject { get; }
    public Vector3 Position { get; }
    public bool IsBroken { get; }

    public ObjectSnapshot(GameObject gameObject, Vector3 position)
    {
        GameObject = gameObject;
        Position = position;
    }

    public ObjectSnapshot(GameObject gameObject, bool isBroken)
    {
        GameObject = gameObject;
        IsBroken = isBroken;
    }
}