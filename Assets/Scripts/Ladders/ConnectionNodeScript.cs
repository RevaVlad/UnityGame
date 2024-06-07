using System.Linq;
using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    private TransparencyControlScript _animation;

    private void Awake()
    {
        _animation = GetComponentsInChildren<TransparencyControlScript>().FirstOrDefault();
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(Utils.LaddersLayerName)) return;
        var thisRoot = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
        var root = Utils.GetPipeRoot(other.transform).GetComponent<LadderScript>();
        if (!root.GetBases().Contains(other.transform.name))
        {
            thisRoot.DestroyConnection(root.transform);
            _animation.Disappear();
        }
        else
        {
            thisRoot.ConnectLadders(root.transform);
            _animation.Appear();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(Utils.LaddersLayerName)) return;
        var root = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
        root.DestroyConnection(Utils.GetPipeRoot(other.transform));
        _animation.Disappear();
    }
}