using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[SelectionBase]
public class LadderScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<LadderScript> connected;
    [field: SerializeField] public int MoveDirection { get; private set; }
    [SerializeField] private bool isFalling = true;

    private readonly List<GetNearbyObjectsScript> rightObjectsCollider = new();
    private readonly List<GetNearbyObjectsScript> leftObjectsCollider = new();
    private readonly List<FallingProcessingScript> downObjectsColliders = new();
    private Coroutine moveCoroutine;
    private Rigidbody2D rb;


    private void Start()
    {
        connected = new List<LadderScript>();
        rb = GetComponent<Rigidbody2D>();

        foreach (Transform child in transform.Find("Tiles"))
        {
            rightObjectsCollider.Add(child.Find("RightCollider").GetComponent<GetNearbyObjectsScript>());
            leftObjectsCollider.Add(child.Find("LeftCollider").GetComponent<GetNearbyObjectsScript>());
            downObjectsColliders.Add(child.Find("DownCollider").GetComponent<FallingProcessingScript>());
        }

        transform.SetParent(GameObject.Find("LaddersContainer") is null
            ? new GameObject("LaddersContainer").transform
            : GameObject.Find("LaddersContainer").transform);
    }

    public void ConnectLadders(Transform other)
    {
        var ladder = other.GetComponent<LadderScript>();
        if (!connected.Contains(ladder))
            connected.Add(other.GetComponent<LadderScript>());
    }

    public void DestroyConnection(Transform other) => connected.Remove(other.GetComponent<LadderScript>());

    private List<GameObject> GetRightCollidingObjects() =>
        rightObjectsCollider.SelectMany(collider => collider.CollidingObjects).ToList();

    private List<GameObject> GetLeftCollidingObjects() =>
        leftObjectsCollider.SelectMany(collider => collider.CollidingObjects).ToList();

    private void MoveNearbyObjects(bool right)
    {
        var laddersOnDirection =
            (right ? GetRightCollidingObjects() : GetLeftCollidingObjects())
            .Where(obj => obj.layer == LayerMask.NameToLayer("Ladders"))
            .Select(obj => PipeUtils.GetPipeRoot(obj.transform));

        foreach (var ladder in laddersOnDirection)
        {
            var script = ladder.GetComponent<LadderScript>();
            if (connected.Contains(script)) continue;
            if (right) script.MoveRight();
            else script.MoveLeft();
        }
    }

    private IEnumerator MoveHorizontalCoroutine(bool right)
    {
        if (MoveDirection != 0 || !CheckIfMoveIsPossible(right))
            yield break;

        MoveDirection = right ? 1 : -1;

        MoveNearbyObjects(right);
        MoveConnected(right);

        var gameObjectTransform = transform;
        var position = gameObjectTransform.position;
        var target = position + new Vector3(MoveDirection, 0, 0);

        rb.velocity = new Vector2(moveSpeed * MoveDirection, 0);
        while ((target.x - gameObjectTransform.position.x) * MoveDirection > 1e-4)
        {
            gameObjectTransform.position = new Vector3(
                gameObjectTransform.position.x + (float)(moveSpeed * MoveDirection * 1e-3),
                position.y, 0);
            rb.MovePosition(new Vector2(position.x + MoveDirection * moveSpeed * Time.smoothDeltaTime, position.y));
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector2.zero;
        MoveDirection = 0;
        transform.position = target;
        rb.MovePosition(target);
        moveCoroutine = null;
    }

    private void MoveConnected(bool right)
    {
        var temp = connected.ToArray();
        foreach (var connectedObject in temp)
        {
            if (connectedObject is null) continue;
            if (!connectedObject.CheckIfMoveIsPossible(right)) continue;
            if (right) connectedObject.MoveRight();
            else connectedObject.MoveLeft();
        }
    }

    private bool CheckIfMoveIsPossible(bool right)
    {
        if (isFalling) return false;
        var objectsAtDirection =
            right ? GetRightCollidingObjects() : GetLeftCollidingObjects();

        if (objectsAtDirection.Any(obj => obj.layer == LayerMask.NameToLayer("Platforms") && !obj.CompareTag("Hidden")))
            return false;

        return objectsAtDirection.Where(obj => obj.layer == LayerMask.NameToLayer("Ladders"))
            .Select(obj => PipeUtils.GetPipeRoot(obj.transform)).All(ladder =>
                ladder.GetComponent<LadderScript>().CheckIfMoveIsPossible(right));
    }

    public bool CheckIfExitAvailable()
    {
        var collider = Physics2D.OverlapCircleAll(transform.Find("ExitPoint").transform.position, 0.1f,
            LayerMask.GetMask("Platforms"));
        return collider.Length == 0;
    }

    [ContextMenu("MoveRight")]
    public void MoveRight() => moveCoroutine ??= StartCoroutine(MoveHorizontalCoroutine(true));

    [ContextMenu("MoveLeft")]
    public void MoveLeft() => moveCoroutine ??= StartCoroutine(MoveHorizontalCoroutine(false));

    public void StopFall()
    {
        isFalling = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        var position = transform.position;
        position = new Vector3(position.x, math.round(position.y * 2) / 2f);
        transform.position = position;
    }

    public void TryStartFall()
    {
        if (downObjectsColliders.Select(script => script.CurrentlyStopping).Sum() > 0) return;
        isFalling = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.WakeUp();
    }

    public IEnumerable<string> GetBases()
    {
        var possibleBase = downObjectsColliders
            .Where(script => script.CurrentlyStopping > 0)
            .Select(script => script.transform.parent)
            .ToArray();
        if (possibleBase.Length == 0)
            return Array.Empty<string>();
        var min = possibleBase.Min(obj => obj.position.y);
        var bases = possibleBase.Where(obj => Math.Abs(obj.position.y - min) < .5).Select(obj => obj.name).ToArray();
        return bases;
    }
}