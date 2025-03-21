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


    private void Awake() =>
        transform.SetParent(GameObject.Find("LaddersContainer") is null
            ? new GameObject("LaddersContainer").transform
            : GameObject.Find("LaddersContainer").transform);

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

    private void MoveNearbyObjects(bool right, LadderScript startedBy)
    {
        var laddersOnDirection =
            (right ? GetRightCollidingObjects() : GetLeftCollidingObjects())
            .Where(obj => obj.layer == LayerMask.NameToLayer(Utils.LaddersLayerName))
            .Select(obj => Utils.GetPipeRoot(obj.transform));

        foreach (var ladder in laddersOnDirection)
        {
            var script = ladder.GetComponent<LadderScript>();
            if (connected.Contains(script)) continue;
            if (right) script.MoveRight(startedBy);
            else script.MoveLeft(startedBy);
        }
    }

    public void StopMoveCoroutine()
    {
        if (moveCoroutine is null) return;
        StopCoroutine(moveCoroutine);
        moveCoroutine = null;
        MoveDirection = 0;
        rb.velocity = Vector2.zero;
    }

    private IEnumerator MoveHorizontalCoroutine(bool right, LadderScript startedBy)
    {
        if (MoveDirection != 0 || !CheckIfMoveIsPossible(right, startedBy))
            yield break;

        MoveDirection = right ? 1 : -1;

        MoveNearbyObjects(right, startedBy);
        MoveConnected(right, startedBy);

        yield return MoveLaddersHorizontalCoroutine();

        StopMoveCoroutine();
    }

    private IEnumerator MoveLaddersHorizontalCoroutine()
    {
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

        transform.position = target;
        rb.MovePosition(target);
    }

    private void MoveConnected(bool right, LadderScript startedBy)
    {
        var temp = connected.ToArray();
        foreach (var connectedObject in temp)
        {
            if (connectedObject is null) continue;
            if (!connectedObject.CheckIfMoveIsPossible(right, startedBy)) continue;
            if (right) connectedObject.MoveRight(startedBy);
            else connectedObject.MoveLeft(startedBy);
        }
    }

    private bool CheckIfMoveIsPossible(bool right, LadderScript startedBy)
    {
        if (isFalling) return false;
        var objectsAtDirection =
            right ? GetRightCollidingObjects() : GetLeftCollidingObjects();

        if (objectsAtDirection.Any(obj =>
                obj.layer == LayerMask.NameToLayer(Utils.PlatformsLayerName) && !obj.CompareTag("Hidden")))
            return false;

        return objectsAtDirection.Where(obj => obj.layer == LayerMask.NameToLayer(Utils.LaddersLayerName))
            .Select(obj => Utils.GetPipeRoot(obj.transform).GetComponent<LadderScript>()).All(ladder =>
                ladder.CheckIfMoveIsPossible(right, startedBy) && !ladder.connected.Contains(startedBy));
    }

    public bool CheckIfExitAvailable()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.Find(Utils.PipeExitPointName).transform.position, 0.1f,
            LayerMask.GetMask(Utils.PlatformsLayerName, Utils.LaddersLayerName));
        return !colliders.Any(collider => collider.gameObject.layer == LayerMask.GetMask(Utils.PlatformsLayerName) ||
                                          LayerMask.GetMask(Utils.PlayerLayerName) != collider.excludeLayers);
    }

    [ContextMenu("MoveRight")]
    public void MoveRight(LadderScript startedBy = null) =>
        moveCoroutine ??= StartCoroutine(MoveHorizontalCoroutine(true, startedBy ?? this));

    [ContextMenu("MoveLeft")]
    public void MoveLeft(LadderScript startedBy = null) =>
        moveCoroutine ??= StartCoroutine(MoveHorizontalCoroutine(false, startedBy ?? this));

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