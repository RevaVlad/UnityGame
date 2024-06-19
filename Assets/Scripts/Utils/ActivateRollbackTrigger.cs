using UnityEngine;

public class ActivateRollbackTrigger : MonoBehaviour
{
    private SceneManager sceneManager;

    public bool IsRollbackActive { get; private set; }

    private void Start() => sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!IsRollbackActive)
            sceneManager.ClearSceneSnapshots();
        IsRollbackActive = !IsRollbackActive;
    }
}