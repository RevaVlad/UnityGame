using System;
using UnityEngine;

public static class Utils
{
    public const string LaddersLayerName = "Ladders";
    public const string PlatformsLayerName = "Platforms";
    public const string PlayerLayerName = "Player";
    public const string PipeEnterPointName = "EnterPoint";
    public const string PipeExitPointName = "ExitPoint";
    public const string PipeTileForConnection = "EnterUp";

    public static Transform GetPipeRoot(Transform obj)
    {
        while (!obj.CompareTag("PipeRoot"))
        {
            if (obj is null) throw new FormatException("Not a pipe");
            obj = obj.parent;
        }

        return obj;
    }
}