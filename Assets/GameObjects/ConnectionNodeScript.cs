using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladders") 
            && (transform.position - other.transform.position).y > 0)
            transform.parent.GetComponent<LadderScript>().ConnectLadders(other);
    }
}
