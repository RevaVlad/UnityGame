using UnityEngine;

public class ActivateRollbackTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(false);
    }
}
