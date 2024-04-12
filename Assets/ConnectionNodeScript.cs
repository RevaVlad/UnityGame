using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionNodeScript : MonoBehaviour
{
    public float force = 0.01f;
    private Rigidbody2D rb;
    
    void Start()
    {
        Debug.Log("Created connection node");
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector2(force, 0));
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        var otherObject = other.gameObject;
        Debug.Log($"Collision!!! with object {otherObject.name}");
        
        if (otherObject.layer == LayerMask.NameToLayer("Ladders"))
        {
            var ladderObject = transform.parent.gameObject;
            Debug.Log($"child: {name}, parent: {ladderObject.name}");
            //Debug.Log("Connected ladders");
            otherObject.transform.position = new Vector2(ladderObject.transform.position.x, other.transform.position.y);

            var joint = ladderObject.GetComponent<FixedJoint2D>();
            joint.enabled = true;
            joint.connectedBody = otherObject.GetComponent<Rigidbody2D>();
        }
    }

}
