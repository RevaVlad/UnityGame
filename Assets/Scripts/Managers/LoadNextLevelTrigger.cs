using UnityEngine;

public class LoadNextLevelScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") &&
            LayerMask.LayerToName(other.gameObject.layer) != Utils.LaddersLayerName) return;
        SceneManager.LoadNextLevel();
    }
}