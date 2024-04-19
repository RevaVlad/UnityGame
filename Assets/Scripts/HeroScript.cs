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
        var position = transform.position;
        position =
            Vector2.MoveTowards(position, new Vector2(position.x, position.y) + direction, speed * Time.deltaTime);
        transform.position = position;
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