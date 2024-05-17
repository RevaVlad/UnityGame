using System;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroScript : MonoBehaviour
{
    [SerializeField] private float jumpForce;

    public PlayerRunData Data;
    
    private bool faceRight;
    private Animator anim;
    private float LastOnGroundTime;
    private bool IsJumping;

    
    private bool isGrounded;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private PlayerControls _playerControls;
    private PlayerInput _playerInput;
    private Vector2 direction;

    [SerializeField] private float sizeY;

    public bool isPlayerOnLadder;
    private LadderScript heldLadder = null;

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
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();

        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions.FindActionMap("BasicInput").Enable();
        _playerInput.actions.FindActionMap("LadderInput").Disable();

        sizeY = GetComponent<Collider2D>().bounds.size.y;
    }

    private void Update()
    {
        LastOnGroundTime += Time.deltaTime;
        CheckGround();
    }

    private void FixedUpdate()
    {
        if (isPlayerOnLadder)
        {
            var ladderEntryPoint = heldLadder.EnterPoint.position;
            var playerCenter = transform.position;
            if ((playerCenter - ladderEntryPoint).magnitude > .01f)
                transform.position = Vector2.MoveTowards(playerCenter, 
                    ladderEntryPoint + new Vector3(0, -.5f + sizeY / 2), 5f * Time.smoothDeltaTime);
        }
        else
        {
            Run();
            ApplyFriction();
        }
    }

    public void OnJump()
    {
        if (LastOnGroundTime < 1e-2)
        {
            if (rb.velocity.magnitude > 0)
                rb.AddForce((faceRight ? 1f : -1f) * Vector2.right * 40f, ForceMode2D.Impulse);
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnMove(InputValue inputValue)
    {
        direction = inputValue.Get<Vector2>();
        if (direction.magnitude > 0)
        {
            //Run();
            anim.SetFloat("moveX", Data.animationSpeed);
            AdjustSprite();
        }
        else 
            anim.SetFloat("moveX", 0);
    }

    private void Run()
    {
        var targetSpeed = direction.x * Data.runMaxSpeed;
        
		var speedDif = targetSpeed - rb.velocity.x;

        // decide which acceleration is used
        var currentAcceleration = !isGrounded
            ? (direction.x != 0 ? Data.airAcceleration : Data.airDecceleration)
            : (direction.x != 0 ? Data.acceleration : Data.decceleration);

        /*
		if(Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
			accelRate = 0; 
        */

		rb.AddForce(Vector2.right * (currentAcceleration * speedDif));
    }

    private void ApplyFriction()
    {
        if (isGrounded && Mathf.Abs(direction.x) < 1e-2f) // if grounded and tries to stop
        {
            var amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(Data.frictionAmount));
            // var amount = Data.frictionAmount;
            rb.AddForce(Vector2.right * (-Mathf.Sign(rb.velocity.x) * amount), ForceMode2D.Impulse);
        }
    }

    private void AdjustSprite()
    {
        if ((!(direction.x > 0) || !faceRight) && (!(direction.x < 0) || faceRight)) return;
        transform.localScale *= new Vector2(-1, 1);
        faceRight = !faceRight;
    }


    private void CheckGround()
    {
        var collider = Physics2D.OverlapCircleAll(transform.position - Vector3.up * sizeY,
            0.05f, LayerMask.GetMask("Platforms", "Ladders"));
        isGrounded = collider.Length > 0;
        if (isGrounded)
            LastOnGroundTime = 0;
        anim.SetBool("isGrounded", isGrounded);
    }

    #region LaddersControls
    private void OnTakeLadder()
    {
        if (!TryGetLadder(out var ladder)) return;
        transform.SetParent(ladder.transform);
        isPlayerOnLadder = true;
        heldLadder = ladder;
        SwapInputMap();
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
    
    private void OnTravelThroughPipe()
    {
        var enter = heldLadder.transform.Find("EnterPoint");
        var distance = (enter.position - transform.position);
        if (distance.magnitude < .2 && heldLadder.CheckIfExitAvailable())
        {
            transform.position = heldLadder.transform.Find("ExitPoint").position;
            OnDropLadder();
        }
        anim.Play("PlayerPipeGo");
    }

    private bool TryGetLadder(out LadderScript ladder)
    {
        ladder = null;
        var collider = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y),
            0.2f, LayerMask.GetMask("Ladders"));
        if (collider.Length == 0) return false;
        ladder = PipeUtils.GetPipeRoot(collider[0].transform).GetComponent<LadderScript>();
        return true;
    }
    #endregion

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