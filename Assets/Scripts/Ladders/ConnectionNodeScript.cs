using System.Linq;
using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    private TransparencyControlScript _animation;
    private LadderScript thisPipe;

    private void Awake()
    { 
        _animation = GetComponent<TransparencyControlScript>();
        thisPipe = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(Utils.LaddersLayerName)) return;
        
        var otherRoot = Utils.GetPipeRoot(other.transform).GetComponent<LadderScript>();
        if (otherRoot != thisPipe && otherRoot.GetBases().Contains(other.transform.name))
        {
            _animation.Appear();
            thisPipe.ConnectLadders(otherRoot.transform);
        }
        else
        {
            _animation.Disappear();
            thisPipe.DestroyConnection(otherRoot.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(Utils.LaddersLayerName)) return;
        var otherRoot = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
        
        _animation.Disappear();
        otherRoot.DestroyConnection(Utils.GetPipeRoot(other.transform));
    }
}