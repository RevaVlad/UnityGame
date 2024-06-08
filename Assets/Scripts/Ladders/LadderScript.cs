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
    private readonly List<FallingProcessingScript> fallingProcessingScripts = new();
    private Coroutine moveCoroutine;
    private Rigidbody2D rb;

    public enum MoveSource
    {
        Left,
        Right,
        Down,
        Player
    }

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
            fallingProcessingScripts.Add(child.Find("DownCollider").GetComponent<FallingProcessingScript>());
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

    private void MoveNearbyLadders(bool right)
    {
        var laddersOnDirection =
            (right ? GetRightCollidingObjects() : GetLeftCollidingObjects())
            .Where(obj => obj.layer == LayerMask.NameToLayer(Utils.LaddersLayerName))
            .Select(obj => Utils.GetPipeRoot(obj.transform)).Distinct();

        foreach (var ladder in laddersOnDirection)
        {
            var script = ladder.GetComponent<LadderScript>();
            if (right) script.MoveRight(MoveSource.Left);
            else script.MoveLeft(MoveSource.Right);
        }
    }

    private void MoveConnected(bool right)
    {
        var temp = connected.ToArray();
        foreach (var connectedObject in temp)
        {
            if (connectedObject is null) continue;
            if (!connectedObject.CheckIfMoveIsPossible(right, MoveSource.Down)) continue;
            if (right) connectedObject.MoveRight(MoveSource.Down);
            else connectedObject.MoveLeft(MoveSource.Down);
        }
    }

    private IEnumerator MoveHorizontalCoroutine(bool right, MoveSource cameFrom)
    {
        if (MoveDirection != 0) yield break;
        MoveDirection = (right) ? 1 : -1;

        if (!CheckIfMoveIsPossible(right, cameFrom))
        {
            MoveDirection = 0;
            yield break;
        }

        MoveNearbyLadders(right);
        MoveConnected(right);

        yield return MoveLaddersHorizontalCoroutine();

        StopMoveCoroutine();
    }

    public void StopMoveCoroutine()
    {
        if (moveCoroutine is null) return;
        StopCoroutine(moveCoroutine);
        moveCoroutine = null;
        MoveDirection = 0;
        rb.velocity = Vector2.zero;
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

    private bool CheckIfMoveIsPossible(bool right, MoveSource cameFrom)
    {
        if (isFalling) return false;
        var objectsAtDirection =
            right ? GetRightCollidingObjects() : GetLeftCollidingObjects();

        if (objectsAtDirection.Any(obj => obj.layer == LayerMask.NameToLayer(Utils.PlatformsLayerName)))
            return false;

        var laddersAtDirection =
            objectsAtDirection.Where(obj => obj.layer == LayerMask.NameToLayer(Utils.LaddersLayerName))
                .Select(obj => Utils.GetPipeRoot(obj.transform).GetComponent<LadderScript>()).ToArray();

        if (cameFrom == MoveSource.Down && laddersAtDirection.Any(ladder => ladder.MoveDirection == 0))
            return false;

        return laddersAtDirection.All(ladder => ladder.CheckIfMoveIsPossible(right, cameFrom));
    }

    public bool CheckIfExitAvailable()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.Find(Utils.PipeExitPointName).transform.position, 0.1f,
            LayerMask.GetMask(Utils.PlatformsLayerName, Utils.LaddersLayerName));
        return !colliders.Any(collider => collider.gameObject.layer == LayerMask.GetMask(Utils.PlatformsLayerName) ||
                                          LayerMask.GetMask(Utils.PlayerLayerName) != collider.excludeLayers);
    }

    [ContextMenu("MoveRight")]
    public void MoveRight(MoveSource cameFrom = MoveSource.Player) =>
        moveCoroutine ??= StartCoroutine(MoveHorizontalCoroutine(true, cameFrom));

    [ContextMenu("MoveLeft")]
    public void MoveLeft(MoveSource cameFrom = MoveSource.Player) =>
        moveCoroutine ??= StartCoroutine(MoveHorizontalCoroutine(false, cameFrom));

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
        if (fallingProcessingScripts.Select(script => script.CurrentlyStoppingCount).Sum() > 0) return;
        isFalling = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.WakeUp();
    }

    public IEnumerable<string> GetBases()
    {
        var possibleBase = fallingProcessingScripts
            .Where(script => script.CurrentlyStoppingCount > 0)
            .Select(script => script.transform.parent)
            .ToArray();
        if (possibleBase.Length == 0)
            return Array.Empty<string>();
        var min = possibleBase.Min(obj => obj.position.y);
        var bases = possibleBase.Where(obj => Math.Abs(obj.position.y - min) < .5).Select(obj => obj.name).ToArray();
        return bases;
    }
}