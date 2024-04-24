using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private LadderScript connected;
    [SerializeField] private LadderScript permanentlyConnected;

    private GetNearbyObjectsScript rightObjectsCollider;
    private GetNearbyObjectsScript leftObjectsCollider;
    
    private int _moveDirection = 0;
    private Coroutine _moveCoroutine = null;
    
    void Start()
    {
        rightObjectsCollider = transform.Find("RightCollider").GetComponent<GetNearbyObjectsScript>();
        leftObjectsCollider = transform.Find("LeftCollider").GetComponent<GetNearbyObjectsScript>();

        if (GameObject.Find("LaddersContainer") is null)
            new GameObject("LaddersContainer");
        
        transform.SetParent(permanentlyConnected is not null
            ? permanentlyConnected.gameObject.transform
            : GameObject.Find("LaddersContainer").transform);
    }
    
    internal void ConnectLadders(Collider2D other)
    {
        var otherTransform = other.transform;
        transform.SetParent(otherTransform);
        connected = otherTransform.GetComponent<LadderScript>();
    }

    internal void DestroyConnection()
    {
        Transform transform1;
        (transform1 = transform).SetParent(GameObject.Find("LaddersContainer").transform);
        connected = null;
        transform.position = new Vector3(math.round(transform1.position.x), transform.position.y, 0);
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
    
    //private MoveConnectedObject()

    private IEnumerator MoveHorizontalCourutine(bool right)
    {
        if (!CheckIfMoveIsPossible(right))
        {
            //DestroyConnection();
            Debug.Log("Failed move");
            yield break;
        }

        MoveNearbyObjects(right);
        
        _moveDirection = (right) ? 1 : -1;
        var position = transform.position;
        transform.position = new Vector3(math.round(position.x), position.y, 0);
        var target = position + new Vector3(_moveDirection, 0, 0);

        var rb = GetComponent<Rigidbody2D>();
        while ((target.x - transform.position.x) * _moveDirection > 1e-4)
        {
            transform.position = new Vector3(transform.position.x + (float)(moveSpeed * _moveDirection * 1e-3), position.y, 0);
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
        if (objectsAtDirection.Any(obj => obj.layer == LayerMask.NameToLayer("Platforms")))
            return false;

        return objectsAtDirection.Where(obj => obj.layer == LayerMask.NameToLayer("Ladders")).All(ladder =>
            ladder.GetComponent<LadderScript>().CheckIfMoveIsPossible(right));
    }

    [ContextMenu("MoveRight")]
    public void MoveRight()
    {
        if (_moveCoroutine is null)
            _moveCoroutine = StartCoroutine(MoveHorizontalCourutine(true));
    }
    
    [ContextMenu("MoveLeft")]
    public void MoveLeft()
    {
        if (_moveCoroutine is null)
            _moveCoroutine = StartCoroutine(MoveHorizontalCourutine(false));
    }
}
