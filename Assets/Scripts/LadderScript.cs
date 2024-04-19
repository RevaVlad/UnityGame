using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private LadderScript connected = null;
    private int _moveDirection = 0;
    private Coroutine _moveCoroutine = null;
    
    void Start()
    {
        transform.SetParent(GameObject.Find("LaddersContainer").transform);
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    internal void ConnectLadders(Collider2D other)
    {
        var otherTransform = other.transform;
        transform.SetParent(otherTransform);
        connected = otherTransform.GetComponent<LadderScript>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Platforms"))
            DestroyConnection();
    }

    internal void DestroyConnection()
    {
        transform.SetParent(GameObject.Find("LaddersContainer").transform);
        connected = null;
        transform.position = new Vector3(math.round(transform.position.x), transform.position.y, 0);
    }

    private IEnumerator MoveHorizontalCourutine(bool right)
    {
        _moveDirection = (right) ? 1 : -1;
        transform.position = new Vector3(math.round(transform.position.x), transform.position.y, 0);
        var target = transform.position + new Vector3(_moveDirection, 0, 0);

        var rb = GetComponent<Rigidbody2D>();
        rb.mass = 1000;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        while ((target.x - transform.position.x) * _moveDirection > 1e-4)
        {
            transform.position = new Vector3(transform.position.x + (float)(moveSpeed * _moveDirection * 1e-3), transform.position.y, 0);
            rb.MovePosition(new Vector2(transform.position.x + _moveDirection * moveSpeed * Time.smoothDeltaTime, transform.position.y));
            yield return new WaitForFixedUpdate();
        }
        
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.position = target;
        rb.MovePosition(target);
        rb.mass = 1;
        _moveCoroutine = null;
        _moveDirection = 0;
    }

    [ContextMenu("MoveRight")]
    private void MoveRight()
    {
        if (_moveCoroutine is null)
            _moveCoroutine = StartCoroutine(MoveHorizontalCourutine(true));
    }
    
    [ContextMenu("MoveLeft")]
    private void MoveLeft()
    {
        if (_moveCoroutine is null)
            _moveCoroutine = StartCoroutine(MoveHorizontalCourutine(false));
    }
}
