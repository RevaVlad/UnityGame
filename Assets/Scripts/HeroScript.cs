using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroScript : MonoBehaviour
{
    [SerializeField] private float speedMidAir;
    [SerializeField] private float speedOnGround;
    [SerializeField] private float jumpForce;


    private bool isGrounded;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private PlayerControls _playerControls;
    private PlayerInput _playerInput;
    private Vector2 direction;

    [SerializeField] private float sizeY;

    public bool isPlayerOnLadder;
    private LadderScript heldLadder = null;

    private Animator anim;
    private bool faceRight;

    private void Start()
    {
        _playerControls = new PlayerControls();
    }

    private void SwapInputMap()
    {
        if (_playerInput.actions.FindActionMap("BasicInput").enabled)
        {
            _playerInput.actions.FindActionMap("LadderInput").Enable();
            _playerInput.actions.FindActionMap("BasicInput").Disable();
        }
        else
        {
            _playerInput.actions.FindActionMap("BasicInput").Enable();
            _playerInput.actions.FindActionMap("LadderInput").Disable();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 20f;
        rb.gravityScale = 1.1f;
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();

        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions.FindActionMap("BasicInput").Enable();
        _playerInput.actions.FindActionMap("LadderInput").Disable();

        sizeY = GetComponent<Collider2D>().bounds.size.y;
    }

    private void Update()
    {
        CheckGround();
    }

    private void FixedUpdate()
    {
        if (isPlayerOnLadder)
        {
            var ladderEntryPoint = heldLadder.EnterPosition.position - new Vector3(0, sizeY / 2); // -sizeY / 2 потому что у Player origin сдвинут
            var playerCenter = transform.position;
            if ((playerCenter - ladderEntryPoint).magnitude > .05f)
            {
                transform.position = Vector2.MoveTowards(playerCenter, ladderEntryPoint, 1.3f * Time.smoothDeltaTime);
            }
        }
        else
        {
            var position = transform.position;
            position =
                Vector2.MoveTowards(position, new Vector2(position.x, position.y) + direction,
                    (isGrounded ? speedOnGround : speedMidAir) * Time.deltaTime);
            transform.position = position;
        }
    }

    public void OnJump()
    {
        if (isGrounded)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnMove(InputValue inputValue)
    {
        direction = inputValue.Get<Vector2>();
        if (direction.magnitude > 0)
        {
            anim.SetFloat("moveX", speedOnGround);
            ReflectForRun();
        }
        else 
            anim.SetFloat("moveX", 0);
    }

    private void ReflectForRun()
    {
        if ((!(direction.x > 0) || !faceRight) && (!(direction.x < 0) || faceRight)) return;
        transform.localScale *= new Vector2(-1, 1);
        faceRight = !faceRight;
    }

    private void OnTravelThroughPipe()
    {
        if (TryTravelThoughPipe())
        {
            var enter = heldLadder.transform.Find("EnterPoint");
            var distance = (enter.position - (transform.position + new Vector3(0, sizeY / 2, 0)));
            if (distance.magnitude < .1 && heldLadder.CheckIfExitAvailable())
            {
                transform.position = heldLadder.transform.Find("ExitPoint").position - new Vector3(0, sizeY / 2, 0);
                OnDropLadder();
            }
            anim.Play("PlayerPipeGo");
        }
    }

    private void CheckGround()
    {
        var collider = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, sizeY / 2, 0), 0.2f, LayerMask.GetMask("Platforms", "Ladders"));
        isGrounded = collider.Length > 0;
        anim.SetBool("isGrounded", isGrounded);
    }

    private void OnTakeLadder()
    {
        if (TryGetLadder(out var ladder))
        {
            transform.SetParent(ladder.transform);
            isPlayerOnLadder = true;
            heldLadder = ladder;
            SwapInputMap();

            // StartCoroutine(MoveToPoint(ladder.transform.position));
        }
    }

    private void OnDropLadder()
    {
        SwapInputMap();
        transform.SetParent(null);
        isPlayerOnLadder = false;
        heldLadder = null;
    }

    private void OnMoveRightWithLadder()
    {
        if (isGrounded)
            heldLadder.MoveRight();
        if (faceRight)
        {
            transform.localScale *= new Vector2(-1, 1);
            faceRight = !faceRight;
        }
        anim.Play("PlayerRun");
    }

    private void OnMoveLeftWithLadder()
    {
        if (isGrounded)
            heldLadder.MoveLeft();
        if (!faceRight)
        {
            transform.localScale *= new Vector2(-1, 1);
            faceRight = !faceRight;
        }
        anim.Play("PlayerRun");
    }

    private bool TryGetLadder(out LadderScript ladder)
    {
        ladder = null;
        var collider = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y + sizeY / 2),
            0.2f, LayerMask.GetMask("Ladders"));
        if (collider.Length == 0) return false;
        ladder = collider[0].gameObject.GetComponent<LadderScript>();
        return true;
    }

    private bool TryTravelThoughPipe()
    {
        var colliderPosition = heldLadder.transform.Find("ExitPoint").position - new Vector3(0, sizeY / 2, 0);
        var collider = Physics2D.OverlapCircleAll(colliderPosition,
            0.2f, LayerMask.GetMask("Platforms"));
        return collider.Where(x => x.transform.name != "HiddenPlatform").ToArray().Length == 0;
    }

    /*
    private IEnumerator MoveToPoint(Vector3 point)
    {
        while ((transform.position - point).magnitude < 1e-3)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                point, 1.3f * Time.smoothDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
    */
}