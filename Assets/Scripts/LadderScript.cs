using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class LadderScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<LadderScript> connected;
    [SerializeField] private int _moveDirection = 0;
    [SerializeField] internal bool isFalling = true;

    public Transform EnterPoint { get; private set; }

    private List<GetNearbyObjectsScript> rightObjectsCollider = new ();
    private List<GetNearbyObjectsScript> leftObjectsCollider = new ();
    private Coroutine _moveCoroutine = null;
    private Rigidbody2D rb;


    private void Start()
    {
        connected = new List<LadderScript>();
        EnterPoint = transform.Find("EnterPoint");
        rb = GetComponent<Rigidbody2D>();
        
        foreach (Transform child in transform.Find("Tiles"))
        {
            rightObjectsCollider.Add(child.Find("RightCollider").GetComponent<GetNearbyObjectsScript>());
            leftObjectsCollider.Add(child.Find("LeftCollider").GetComponent<GetNearbyObjectsScript>());
        }

        if (GameObject.Find("LaddersContainer") is null)
            new GameObject("LaddersContainer");

        transform.SetParent(GameObject.Find("LaddersContainer").transform);
    }

    internal void ConnectLadders(Transform other)
    {
        var ladder = other.GetComponent<LadderScript>();
        if (!connected.Contains(ladder))
            connected.Add(other.GetComponent<LadderScript>());
    }

    internal void DestroyConnection(Transform other)
    {
        // connected.gameObject.transform.Find("HiddenPlatform").GetComponent<BoxCollider2D>().enabled = true;
        connected.Remove(other.GetComponent<LadderScript>());
    }

    private List<GameObject> GetRightCollidingObjects() =>
        rightObjectsCollider.SelectMany(collider => collider.collidingObjects).ToList();
    
    private List<GameObject> GetLeftCollidingObjects() =>
        leftObjectsCollider.SelectMany(collider => collider.collidingObjects).ToList();

    private void MoveNearbyObjects(bool right)
    {
        var laddersOnDirection =
            (right ? GetRightCollidingObjects() : GetLeftCollidingObjects()).Where(
                obj => obj.layer == LayerMask.NameToLayer("Ladders")).Select(obj => PipeUtils.GetPipeRoot(obj.transform));
        
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
        if (_moveDirection != 0 || !CheckIfMoveIsPossible(right))
        {
            // Debug.Log("Failed move");
            yield break;
        }
        
        _moveDirection = (right) ? 1 : -1;

        MoveNearbyObjects(right);
        MoveConnected(right);

        var position = transform.position;
        var target = position + new Vector3(_moveDirection, 0, 0);

        rb.velocity = new Vector2(moveSpeed * _moveDirection, 0);
        while ((target.x - transform.position.x) * _moveDirection > 1e-4)
        {
            transform.position = new Vector3(transform.position.x + (float)(moveSpeed * _moveDirection * 1e-3),
                position.y, 0);
            rb.MovePosition(new Vector2(position.x + _moveDirection * moveSpeed * Time.smoothDeltaTime, position.y));
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector2.zero;

        _moveDirection = 0;
        transform.position = target;
        rb.MovePosition(target);
        _moveCoroutine = null;
    }

    private void MoveConnected(bool right)
    {
        var temp = connected.ToArray();
        foreach (var connectedObject in temp)
        {
            if (connectedObject is null) continue;
            if (!connectedObject.CheckIfMoveIsPossible(right)) {}
            //DestroyConnection(connectedObject.transform);
            else
            {
                if (right) connectedObject.MoveRight();
                else connectedObject.MoveLeft();
            }
        }
    }

    public bool CheckIfMoveIsPossible(bool right)
    {
        if (isFalling) return false;
        var objectsAtDirection =
            (right) ? GetRightCollidingObjects() : GetLeftCollidingObjects();
        
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