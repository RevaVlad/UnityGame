using System;
using UnityEngine;

public static class PipeUtils
{
    public const string LaddersLayerName = "Ladders";

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