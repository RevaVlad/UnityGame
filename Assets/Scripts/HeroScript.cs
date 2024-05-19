using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroScript : MonoBehaviour
{
    public PlayerRunData Data;
    
    private bool faceRight;
    private Animator anim;
    [SerializeField]private float timePassedSinceOnGround;
    [SerializeField]private float timePassedSinceJump;
    
    [SerializeField] private bool isGrounded;
    private bool isJumping = true;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private PlayerControls _playerControls;
    private PlayerInput _playerInput;
    private Vector2 direction;

    [SerializeField] private float sizeY;
    [SerializeField] private float sizeX;

    public bool isPlayerOnLadder;
    private LadderScript heldLadder = null;
    private Coroutine moveToLadderCenter = null;
    
    # region Sounds
    [SerializeField] private AudioClip[] jumpSound;
    [SerializeField] private AudioClip[] pipeSound;
    #endregion

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

        var bounds = GetComponent<CapsuleCollider2D>().bounds;
        (sizeX, sizeY) = (bounds.size.x, bounds.size.y);
    }

    private void Update()
    {
        timePassedSinceOnGround += Time.deltaTime;
        timePassedSinceJump += Time.deltaTime;

        rb.gravityScale = (rb.velocity.y < 0) ? Data.gravityScaleWhenFalling : Data.normalGravityScale;
        CheckGround();
    }

    private void FixedUpdate()
    {
        if (isPlayerOnLadder)
        {
            /*
            var ladderEntryPoint = heldLadder.EnterPoint.position;
            var playerCenter = transform.position;
            transform.position = Vector2.MoveTowards(playerCenter,
                ladderEntryPoint, 100000f * Time.smoothDeltaTime);
            */
        }
        else
        {
            CheckJumpBuffer();
            Run();
            ApplyFriction();
        }
    }

    #region Jump
    public void OnJumpStart()
    {
        if (!isJumping)
            timePassedSinceJump = 0;
    }

    public void OnJumpEnd()
    {
        JumpCut();
    } 

    private void Jump()
    {
        if (rb.velocity.magnitude > 0)
            rb.AddRelativeForce(Vector2.right * (- rb.velocity.x * rb.mass * Data.momentumLossAtJump), ForceMode2D.Force);
        isJumping = true;
        SoundFXManager.instance.PlaySoundFXClip(jumpSound, transform, 1f);
        rb.AddForce(transform.up * (Data.jumpForce - rb.velocity.y * rb.mass), ForceMode2D.Impulse);
    }

    private void JumpCut()
    {
        if (rb.velocity.y > 0 && isJumping)
        {
            rb.AddForce(Vector2.down * rb.velocity.y * Data.jumpCutMultiplier, ForceMode2D.Impulse);
        }
    }

    private void CheckJumpBuffer()
    {
        if (timePassedSinceOnGround < Data.coyoteTime && timePassedSinceJump < Data.bufferTime && !isJumping)
        {
            Jump();
        }
    }
    #endregion

    #region Run
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
    #endregion

    private Collider2D[] results = new Collider2D[1];
    private void CheckGround()
    {
        var size = Physics2D.OverlapCircleNonAlloc(transform.position - Vector3.up * sizeY / 2, sizeX * .3f, results, LayerMask.GetMask("Platforms", "Ladders"));
        isGrounded = size > 0;
        if (isGrounded)
        {
            timePassedSinceOnGround = 0;
            if (timePassedSinceJump > .2) isJumping = false;
        }
        anim.SetBool("isGrounded", isGrounded);
    }

    #region LaddersControls
    
    private void OnTakeLadder()
    {
        if (!TryGetLadder(out var ladder)) return;
        
        transform.SetParent(ladder.transform);
        isPlayerOnLadder = true;
        heldLadder = ladder;
        rb.simulated = false;
        moveToLadderCenter = StartCoroutine(MoveToPoint(ladder.transform, 1.5f, Vector3.up * (sizeY / 2 - .5f)));
        SwapInputMap();
    }

    private void OnDropLadder()
    {
        SwapInputMap();
        transform.SetParent(null);
        isPlayerOnLadder = false;
        heldLadder = null;
        rb.simulated = true;
        if (moveToLadderCenter != null)
        {
            StopCoroutine(moveToLadderCenter);
            moveToLadderCenter = null;
        }
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
        SoundFXManager.instance.PlaySoundFXClip(pipeSound, transform, 1f);
        if (distance.magnitude < .2 && heldLadder.CheckIfExitAvailable())
        {
            transform.position = heldLadder.transform.Find("ExitPoint").position - (sizeY / 2) * Vector3.up;
            OnDropLadder();
        }
        anim.Play("PlayerPipeGo");
    }

    private bool TryGetLadder(out LadderScript ladder)
    {
        ladder = null;
        var collider = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y),
            0.01f, LayerMask.GetMask("Ladders"));
        if (collider.Length == 0) return false;
        ladder = PipeUtils.GetPipeRoot(collider[0].transform).GetComponent<LadderScript>();
        return true;
    }
    #endregion

    private IEnumerator MoveToPoint(Transform target, float speed, Vector3 targetOffset)
    {
        while ((transform.position - target.position).magnitude > 1e-3)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                target.position + targetOffset, speed * Time.smoothDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
}