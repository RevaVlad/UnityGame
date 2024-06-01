using System.Linq;
using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(PipeUtils.LaddersLayerName)) return;
        var thisRoot = PipeUtils.GetPipeRoot(transform).GetComponent<LadderScript>();
        var root = PipeUtils.GetPipeRoot(other.transform).GetComponent<LadderScript>();
        if (!root.GetBases().Contains(other.transform.name))
            thisRoot.DestroyConnection(root.transform);
        else
            thisRoot.ConnectLadders(root.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(PipeUtils.LaddersLayerName)) return;
        var root = PipeUtils.GetPipeRoot(transform).GetComponent<LadderScript>();
        root.DestroyConnection(PipeUtils.GetPipeRoot(other.transform));
    }
}