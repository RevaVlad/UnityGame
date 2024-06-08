using UnityEngine;

public class FallingProcessingScript : MonoBehaviour
{
    private LadderScript _parentScript;
    private GetNearbyObjectsScript _colliderHandler;
    private int _laddersStoppingCount;
    
    public int CurrentlyStoppingCount => _colliderHandler.CollidingObjects.Count;
    public bool AnyLaddersStopping => _laddersStoppingCount > 0;

    private void Start()
    {
        _colliderHandler = GetComponent<GetNearbyObjectsScript>();

        _colliderHandler.EnterCollider.AddListener(OnAddingObject);
        _colliderHandler.ExitCollider.AddListener(OnDeletingObject);

        _parentScript = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
    }

    private void OnAddingObject(string layer)
    {
        if (CurrentlyStoppingCount == 1)
            _parentScript.StopFall();
        if (layer == Utils.LaddersLayerName)
            _laddersStoppingCount++;
    }

    private void OnDeletingObject(string layer)
    {
        if (CurrentlyStoppingCount == 0)
            _parentScript.TryStartFall();  
        if (layer == Utils.LaddersLayerName)
            _laddersStoppingCount--;
    } 
}