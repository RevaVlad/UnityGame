using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    private bool isTriggerWithSlimPlatform;

    private void Start()
    {
        isTriggerWithSlimPlatform = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("SlimPlatform"))
            isTriggerWithSlimPlatform = true;
        if (!isTriggerWithSlimPlatform && other.gameObject.layer == LayerMask.NameToLayer("Ladders"))
            transform.parent.GetComponent<LadderScript>().ConnectLadders(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("SlimPlatform"))
            isTriggerWithSlimPlatform = false;
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladders"))
            transform.parent.GetComponent<LadderScript>().DestroyConnection();
    }
}