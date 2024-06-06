using UnityEngine;

public class FallingProcessingScript : MonoBehaviour
{
    private LadderScript parentScript;
    [field: SerializeField] public int CurrentlyStopping { get; private set; }

    private void Start()
    {
        var getObjectsScript = GetComponent<GetNearbyObjectsScript>();

        getObjectsScript.EnterCollider.AddListener(OnAddingObject);
        getObjectsScript.ExitCollider.AddListener(OnDeletingObject);

        parentScript = Utils.GetPipeRoot(transform).GetComponent<LadderScript>();
    }

    private void OnAddingObject()
    {
        CurrentlyStopping++;
        if (CurrentlyStopping == 1)
            parentScript.StopFall();  
    }

    private void OnDeletingObject()
    {
        CurrentlyStopping--;
        if (CurrentlyStopping == 0)
            parentScript.TryStartFall();  
    } 
}