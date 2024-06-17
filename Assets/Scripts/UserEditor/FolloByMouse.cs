using System;
using UnityEngine;

public class FollowByMouse : MonoBehaviour
{
    private void Update()
    {
        var screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = new Vector3((float)Math.Round(worldPosition.x), (float)Math.Round(worldPosition.y) - 0.5f, 0);
    }
}