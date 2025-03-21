using System.Collections;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroScript : MonoBehaviour
{
    public PlayerRunData data;

    private bool faceRight;
    private Animator anim;
    [SerializeField] private float timePassedSinceOnGround;
    [SerializeField] private float timePassedSinceJump;

    [SerializeField] private bool isGrounded;
    private bool isJumping = true;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Vector2 direction;

    [SerializeField] private float sizeY;
    [SerializeField] private float sizeX;

    private bool isPlayerOnLadder;
    private LadderScript heldLadder;
    private Coroutine moveToLadderCenter;
    private readonly Collider2D[] results = new Collider2D[1];

    # region Sounds

    [SerializeField] private AudioClip[] jumpSound;
    [SerializeField] private AudioClip[] pipeSound;

    #endregion

    [SerializeField] private GameObject shakeManager;
    private static readonly int MoveX = Animator.StringToHash("moveX");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int IsWithPipe = Animator.StringToHash("isWithPipe");
    private static readonly int GoThrowPipe = Animator.StringToHash("goThrowPipes");
    private SpriteRenderer spriteRenderer;

    private void SwapInputMap()
    {
        if (playerInput.actions.FindActionMap("BasicInput").enabled)
        {
            playerInput.actions.FindActionMap("LadderInput").Enable();
            playerInput.actions.FindActionMap("BasicInput").Disable();
        }
        else
        {
            playerInput.actions.FindActionMap("BasicInput").Enable();
            playerInput.actions.FindActionMap("LadderInput").Disable();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("BasicInput").Enable();
        playerInput.actions.FindActionMap("LadderInput").Disable();
        spriteRenderer = GetComponent<SpriteRenderer>();

        var bounds = GetComponent<CapsuleCollider2D>().bounds;
        (sizeX, sizeY) = (bounds.size.x, bounds.size.y);
    }

    private void Update()
    {
        timePassedSinceOnGround += Time.deltaTime;
        timePassedSinceJump += Time.deltaTime;

        rb.gravityScale = rb.velocity.y < 0 ? data.gravityScaleWhenFalling : data.normalGravityScale;
        CheckGround();
    }

    private void FixedUpdate()
    {
        if (isPlayerOnLadder)
            return;
        CheckJumpBuffer();
        Run();
        ApplyFriction();
    }

    #region Jump

    public void OnJumpStart()
    {
        if (!isJumping)
            timePassedSinceJump = 0;
    }

    public void OnJumpEnd() => JumpCut();

    private void Jump()
    {
        if (rb.velocity.magnitude > 0)
            rb.AddRelativeForce(Vector2.right * (-rb.velocity.x * rb.mass * data.momentumLossAtJump),
                ForceMode2D.Force);
        isJumping = true;
        SoundFXManager.Instance.PlaySoundFXClip(jumpSound, transform, 1f);
        rb.AddForce(transform.up * (data.jumpForce - rb.velocity.y * rb.mass), ForceMode2D.Impulse);
    }

    private void JumpCut()
    {
        if (rb.velocity.y > 0 && isJumping)
            rb.AddForce(Vector2.down * rb.velocity.y * data.jumpCutMultiplier, ForceMode2D.Impulse);
    }

    private void CheckJumpBuffer()
    {
        if (timePassedSinceOnGround < data.coyoteTime && timePassedSinceJump < data.bufferTime && !isJumping)
            Jump();
    }

    #endregion

    #region Run

    private void OnMove(InputValue inputValue)
    {
        direction = inputValue.Get<Vector2>();
        if (direction.magnitude > 0)
        {
            anim.SetFloat(MoveX, data.animationSpeed);
            AdjustSprite();
        }
        else
            anim.SetFloat(MoveX, 0);
    }

    private void Run()
    {
        var targetSpeed = direction.x * data.runMaxSpeed;
        var speedDif = targetSpeed - rb.velocity.x;

        var currentAcceleration = !isGrounded
            ? direction.x != 0 ? data.AirAcceleration : data.AirDecceleration
            : direction.x != 0
                ? data.acceleration
                : data.decceleration;

        rb.AddForce(Vector2.right * (currentAcceleration * speedDif));
    }

    private void ApplyFriction()
    {
        if (!isGrounded || !(Mathf.Abs(direction.x) < 1e-2f)) return;
        var amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(data.frictionAmount));
        rb.AddForce(Vector2.right * (-Mathf.Sign(rb.velocity.x) * amount), ForceMode2D.Impulse);
    }

    private void AdjustSprite()
    {
        if ((!(direction.x > 0) || !faceRight) && (!(direction.x < 0) || faceRight)) return;
        transform.localScale *= new Vector2(-1, 1);
        faceRight = !faceRight;
    }

    #endregion

    private void CheckGround()
    {
        var size = Physics2D.OverlapCircleNonAlloc(transform.position - Vector3.up * sizeY / 2, sizeX * .3f, results,
            LayerMask.GetMask(Utils.PlatformsLayerName, Utils.LaddersLayerName));
        isGrounded = size > 0;
        if (isGrounded)
        {
            timePassedSinceOnGround = 0;
            if (timePassedSinceJump > .2) isJumping = false;
        }

        anim.SetBool(IsGrounded, isGrounded);
    }

    #region LaddersControls

    private void OnTakeLadder()
    {
        if (!TryGetLadder(out var ladder)) return;

        GameObject.Find("SceneManager").GetComponent<SceneManager>().CreateObjectsSnapshot();
        transform.SetParent(ladder.transform);
        isPlayerOnLadder = true;
        heldLadder = ladder;
        rb.simulated = false;
        moveToLadderCenter = StartCoroutine(MoveToPoint(ladder.transform, 1.5f, Vector3.up * (sizeY / 2 - .5f)));
        anim.SetBool(IsWithPipe, true);
        SwapInputMap();
    }

    public void OnDropLadder()
    {
        if (!isPlayerOnLadder)
            return;
        SwapInputMap();
        transform.SetParent(null);
        isPlayerOnLadder = false;
        heldLadder = null;
        rb.simulated = true;
        if (moveToLadderCenter == null) return;
        StopCoroutine(moveToLadderCenter);
        anim.SetBool(IsWithPipe, false);
        moveToLadderCenter = null;
    }

    private void OnMoveRightWithLadder()
    {
        if (heldLadder.GetBases().Contains(Utils.PipeTileForConnection))
            heldLadder.MoveRight();
        if (!faceRight) return;
        transform.localScale *= new Vector2(-1, 1);
        faceRight = !faceRight;
    }

    private void OnMoveLeftWithLadder()
    {
        if (heldLadder.GetBases().Contains(Utils.PipeTileForConnection))
            heldLadder.MoveLeft();
        if (faceRight) return;
        transform.localScale *= new Vector2(-1, 1);
        faceRight = !faceRight;
    }

    private void OnTravelThroughPipe()
    {
        var enter = heldLadder.transform.Find(Utils.PipeEnterPointName);
        var gameObjectTransform = transform;
        var distance = enter.position - gameObjectTransform.position;
        if (distance.magnitude < .2 && heldLadder.MoveDirection == 0 && heldLadder.CheckIfExitAvailable())
            StartCoroutine(OnTravelAction());
    }

    private IEnumerator OnTravelAction()
    {
        playerInput.actions.FindActionMap("LadderInput").Disable();
        anim.SetBool(GoThrowPipe, true);
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).IsName("goIntoPipe") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        SoundFXManager.Instance.PlaySoundFXClip(pipeSound, transform, 1f);
        shakeManager.transform.GetComponent<CameraShakeManager>().CameraShake(GetComponent<CinemachineImpulseSource>());
        anim.SetBool(GoThrowPipe, false);
        transform.position = heldLadder.transform.Find(Utils.PipeExitPointName).position - (sizeY / 2) * Vector3.up;
        OnDropLadder();
        playerInput.actions.FindActionMap("BasicInput").Disable();
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerGoOut") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        playerInput.actions.FindActionMap("BasicInput").Enable();
    }

    private void SwitchTransparency()
    {
        spriteRenderer.enabled = spriteRenderer.enabled != true;
    }

    private bool TryGetLadder(out LadderScript ladder)
    {
        ladder = null;
        var position = transform.position;
        var collidedObj = Physics2D.OverlapCircleAll(new Vector2(position.x, position.y),
            0.01f, LayerMask.GetMask(Utils.LaddersLayerName));
        if (collidedObj.Length == 0) return false;
        ladder = Utils.GetPipeRoot(collidedObj[0].transform).GetComponent<LadderScript>();
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