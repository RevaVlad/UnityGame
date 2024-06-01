using UnityEngine;

public class FallingProcessingScript : MonoBehaviour
{
    private LadderScript parentScript;
    [field: SerializeField] public int CurrentlyStopping { get; private set; }

    private void Start()
    {
        var getObjectsScript = GetComponent<GetNearbyObjectsScript>();

        getObjectsScript.AddedObject.AddListener(OnAddingObject);
        getObjectsScript.DeletedObject.AddListener(OnDeletingObject);

        parentScript = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
    }

    private void OnAddingObject(int newCount)
    {
        CurrentlyStopping = newCount;
        if (newCount == 1)
            parentScript.StopFall();
    }

    private void OnDeletingObject(int newCount)
    {
        CurrentlyStopping = newCount;
        if (newCount == 0)
            parentScript.TryStartFall();
    }
}