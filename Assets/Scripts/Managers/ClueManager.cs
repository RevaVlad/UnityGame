using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    [SerializeField] private GameObject _Pipe;
    [SerializeField] private GameObject _PipeSprite;
    [SerializeField] private Vector3 _position;
    [SerializeField] private float moveDuration = 2.0f;

    public void UseClue()
    {
        var spriteRenderers = _Pipe.GetComponentsInChildren<SpriteRenderer>();
        
        foreach (var spriteRenderer in spriteRenderers)
            spriteRenderer.color = Color.red;
        
        var spawnedObject = Instantiate(_PipeSprite, _Pipe.transform.position, Quaternion.identity);
        StartCoroutine(MoveToPosition(spawnedObject, _position, moveDuration));
    }

    private IEnumerator MoveToPosition(GameObject obj, Vector3 target, float duration)
    {
        var startPosition = obj.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = target;
        
        var spriteScript = obj.GetComponent<TransparencyControlScript>();
        if (spriteScript != null)
        {
            spriteScript.Disappear();
            yield return new WaitForSeconds(Utils.TransparencyDisappearTime);
        }
        
        Destroy(obj);
        
        foreach (var spriteRenderer in _Pipe.GetComponentsInChildren<SpriteRenderer>())
            spriteRenderer.color = Color.white;
    }
}