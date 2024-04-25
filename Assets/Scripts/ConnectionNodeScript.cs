using System;
using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladders"))
            transform.parent.GetComponent<LadderScript>().ConnectLadders(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladders"))
            transform.parent.GetComponent<LadderScript>().DestroyConnection();
    }
}