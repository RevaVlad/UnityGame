using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    private bool isTriggerWithSlimPlatform;

    private void Start()
    {
        isTriggerWithSlimPlatform = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //if (other.gameObject.CompareTag("SlimPlatform"))
        //    isTriggerWithSlimPlatform = true;
        if (!isTriggerWithSlimPlatform && other.gameObject.layer == LayerMask.NameToLayer("Ladders"))
        {
            PipeUtils.GetPipeRoot(transform).GetComponent<LadderScript>().ConnectLadders(PipeUtils.GetPipeRoot(other.transform));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if (other.gameObject.CompareTag("SlimPlatform"))
        //    isTriggerWithSlimPlatform = false;
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladders"))
        {
            PipeUtils.GetPipeRoot(transform).GetComponent<LadderScript>().DestroyConnection(PipeUtils.GetPipeRoot(other.transform));
        }
    }
}