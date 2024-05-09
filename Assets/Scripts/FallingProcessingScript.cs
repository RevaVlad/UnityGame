using UnityEngine;

public class FallingProcessingScript : MonoBehaviour
{
    private LadderScript parentScript;
    
    private void Start()
    {
        var getObjectsScript = GetComponent<GetNearbyObjectsScript>();
        
        getObjectsScript.AddedObject.AddListener(OnAddingObject);
        getObjectsScript.DeletedObject.AddListener(OnDeletingObject);
        
        parentScript = PipeUtils.GetPipeRoot(transform).GetComponent<LadderScript>();
        parentScript.isFalling = true;
    }

    private void OnAddingObject(int newCount)
    {
        if (newCount == 1)
            parentScript.StopFall();
    }

    private void OnDeletingObject(int newCount)
    {
        if (newCount == 0)
            parentScript.StartFall();
    }
}
