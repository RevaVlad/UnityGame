using UnityEngine;

public class FallingProcessingScript : MonoBehaviour
{
    private LadderScript parentScript;
    [SerializeField] public int currentlyStopping = 0;
    
    private void Start()
    {
        var getObjectsScript = GetComponent<GetNearbyObjectsScript>();
        
        getObjectsScript.AddedObject.AddListener(OnAddingObject);
        getObjectsScript.DeletedObject.AddListener(OnDeletingObject);
        
        parentScript = PipeUtils.GetPipeRoot(transform).GetComponent<LadderScript>();
    }

    private void OnAddingObject(int newCount)
    {
        currentlyStopping = newCount;
        if (newCount == 1)
            parentScript.StopFall();
    }

    private void OnDeletingObject(int newCount)
    {
        currentlyStopping = newCount;
        if (newCount == 0)
            parentScript.TryStartFall();
    }
}
