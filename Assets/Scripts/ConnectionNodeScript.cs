using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladders")
            && (transform.position - other.transform.position).y > 0)
            transform.parent.GetComponent<LadderScript>().ConnectLadders(other);
        else if (other.gameObject.layer == LayerMask.NameToLayer("Platforms") && !other.gameObject.CompareTag("Hidden"))
            transform.parent.GetComponent<LadderScript>().DestroyConnection();
    }
}