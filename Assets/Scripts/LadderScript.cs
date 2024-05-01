using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[SelectionBase]
public class LadderScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private LadderScript connected;
    [SerializeField] private int _moveDirection = 0;
    [SerializeField] internal bool isFalling = true;

    public Transform EnterPosition { get; private set; }

    private GetNearbyObjectsScript rightObjectsCollider;
    private GetNearbyObjectsScript leftObjectsCollider;
    private Coroutine _moveCoroutine = null;
    private Rigidbody2D rb;


    private void Start()
    {
        rightObjectsCollider = transform.Find("RightCollider").GetComponent<GetNearbyObjectsScript>();
        leftObjectsCollider = transform.Find("LeftCollider").GetComponent<GetNearbyObjectsScript>();
        EnterPosition = transform.Find("EnterPoint").transform;

        if (GameObject.Find("LaddersContainer") is null)
            new GameObject("LaddersContainer");

        transform.SetParent(GameObject.Find("LaddersContainer").transform);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    internal void ConnectLadders(Collider2D other)
    {
        var otherTransform = other.transform;
        connected = otherTransform.GetComponent<LadderScript>();
    }

    internal void DestroyConnection()
    {
        if (connected is not null)
        {
            // connected.gameObject.transform.Find("HiddenPlatform").GetComponent<BoxCollider2D>().enabled = true;
            connected = null;
        }
    }

    private void MoveNearbyObjects(bool right)
    {
        var laddersOnDirection =
            ((right) ? rightObjectsCollider.collidingObjects : leftObjectsCollider.collidingObjects).Where(
                obj => obj.layer == LayerMask.NameToLayer("Ladders"));
        foreach (var ladder in laddersOnDirection)
        {
            if (right)
                ladder.GetComponent<LadderScript>().MoveRight();
            else
                ladder.GetComponent<LadderScript>().MoveLeft();
        }
    }

    private IEnumerator MoveHorizontalCoroutine(bool right)
    {
        if (!CheckIfMoveIsPossible(right))
        {
            //DestroyConnection();
            // Debug.Log("Failed move");
            yield break;
        }

        MoveNearbyObjects(right);
        if (connected is null || !connected.CheckIfMoveIsPossible(right))
            DestroyConnection();
        else
        {
            if (right)
                connected.MoveRight();
            else
                connected.MoveLeft();
        }


        _moveDirection = (right) ? 1 : -1;
        var position = transform.position;
        transform.position = new Vector3(math.round(position.x * 2) / 2f, position.y, 0);
        var target = position + new Vector3(_moveDirection, 0, 0);

        var rb = GetComponent<Rigidbody2D>();
        while ((target.x - transform.position.x) * _moveDirection > 1e-4)
        {
            transform.position = new Vector3(transform.position.x + (float)(moveSpeed * _moveDirection * 1e-3),
                position.y, 0);
            rb.MovePosition(new Vector2(position.x + _moveDirection * moveSpeed * Time.smoothDeltaTime, position.y));
            yield return new WaitForFixedUpdate();
        }

        transform.position = target;
        rb.MovePosition(target);
        _moveCoroutine = null;
    }

    public bool CheckIfMoveIsPossible(bool right)
    {
        var objectsAtDirection =
            (right) ? rightObjectsCollider.collidingObjects : leftObjectsCollider.collidingObjects;
        var collider = Physics2D.OverlapCircleAll(
            new Vector2(transform.position.x, (transform.position.y - transform.localScale.y / 2) -1f), 0.3f,
            LayerMask.GetMask("Ladders"));
        
        var platformsCollider = Physics2D.OverlapCircleAll(
            new Vector2(EnterPosition.transform.position.x, EnterPosition.transform.position.y - 1f / 2), 0.3f,
            LayerMask.GetMask("Platforms"));
        var slimPlatforms = platformsCollider.Where(x => x.CompareTag("SlimPlatform")).ToArray();
        if (slimPlatforms.Length != 0 && collider.Length != 0)
            return false;
        
        if (objectsAtDirection.Any(obj => obj.layer == LayerMask.NameToLayer("Platforms") && !obj.CompareTag("Hidden")))
            return false;

        return objectsAtDirection.Where(obj => obj.layer == LayerMask.NameToLayer("Ladders")).All(ladder =>
            ladder.GetComponent<LadderScript>().CheckIfMoveIsPossible(right));
    }

    public bool CheckIfExitAvailable()
    {
        var collider = Physics2D.OverlapCircleAll(transform.Find("ExitPoint").transform.position, 0.1f,
            LayerMask.GetMask("Platforms"));
        return collider.Length == 0;
    }

    [ContextMenu("MoveRight")]
    public void MoveRight()
    {
        if (_moveCoroutine is null)
            _moveCoroutine = StartCoroutine(MoveHorizontalCoroutine(true));
    }

    [ContextMenu("MoveLeft")]
    public void MoveLeft()
    {
        if (_moveCoroutine is null)
            _moveCoroutine = StartCoroutine(MoveHorizontalCoroutine(false));
    }

    internal void StopFall()
    {
        isFalling = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        transform.position = new Vector3(transform.position.x, math.round(transform.position.y * 2) / 2f);
    }

    internal void StartFall()
    {
        isFalling = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.WakeUp();
    }
}