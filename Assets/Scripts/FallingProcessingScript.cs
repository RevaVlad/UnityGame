using UnityEngine;

public class FallingProcessingScript : MonoBehaviour
{
    private LadderScript parentScript;
    [field: SerializeField] public int CurrentlyStopping { get; private set; }

    private void Start()
    {
        var getObjectsScript = GetComponent<GetNearbyObjectsScript>();

        getObjectsScript.AnyCollisionStarted.AddListener(OnAddingObject);
        getObjectsScript.NoCollisions.AddListener(OnDeletingObject);

        parentScript = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
    }

    private void OnAddingObject() => parentScript.StopFall();

    private void OnDeletingObject() => parentScript.TryStartFall();
}