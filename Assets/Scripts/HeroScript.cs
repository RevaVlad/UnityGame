using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    

    private bool isGrounded;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private PlayerControls _playerControls;
    private Vector2 direction = new ();

    private void Start()
    {
        _playerControls = new PlayerControls();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 20f;
        rb.gravityScale = 1.1f;
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    
    private void Update()
    {
        CheckGround();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }

    private void Run()
    {
        /*
        var direction = transform.right * Input.GetAxis("Horizontal");
        transform.position =
            Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        sprite.flipX = direction.x < 0.0f;
        */
    }

    public void OnJump()
    {
        if (isGrounded)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnMove(InputValue inputValue)
    {
        direction = inputValue.Get<Vector2>();
    }

    private void CheckGround()
    {
        var collider = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        isGrounded = collider.Length > 1;
    }
    
}