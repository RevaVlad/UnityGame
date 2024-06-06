using System.Linq;
using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(Utils.LaddersLayerName)) return;
        var thisRoot = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
        var root = Utils.GetPipeRoot(other.transform).GetComponent<LadderScript>();
        if (!root.GetBases().Contains(other.transform.name))
            thisRoot.DestroyConnection(root.transform);
        else
            thisRoot.ConnectLadders(root.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(Utils.LaddersLayerName)) return;
        var root = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
        root.DestroyConnection(Utils.GetPipeRoot(other.transform));
    }
}