using System;
using UnityEngine;

public class HeroScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    

    private bool isGrounded;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 20f;
        rb.gravityScale = 1.1f;
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (Input.GetButton("Horizontal"))
            Run();
        CheckGround();
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (isGrounded && Input.GetButton("Jump"))
            Jump();
    }

    private void Run()
    {
        var direction = transform.right * Input.GetAxis("Horizontal");
        transform.position =
            Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        sprite.flipX = direction.x < 0.0f;
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        
    }

    private void CheckGround()
    {
        var collider = Physics2D.OverlapCircleAll(transform.position, 0.2f, LayerMask.GetMask("Platforms"));
        isGrounded = collider.Length > 0;
    }
    
}