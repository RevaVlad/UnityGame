using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TransparencyControlScript : MonoBehaviour
{
    [SerializeField] public bool startTransparent = false;
    [SerializeField] private float appearTime = .3f;
    [SerializeField] private float disappearTime = .3f;

    private Material[] _materials;
    
    private void Start()
    {
        _materials = GetComponentsInChildren<SpriteRenderer>().Select(rend => rend.material).ToArray();
        
        if (startTransparent)
            foreach (var material in _materials)
                SetAlpha(material, 0);
    }

    public void Appear() => StartCoroutine(InterpolateTransparencyCoroutine(0, 1, appearTime));

    public void Disappear() => StartCoroutine(InterpolateTransparencyCoroutine(1, 0, disappearTime));
    
    private IEnumerator InterpolateTransparencyCoroutine(float start, float end, float duration)
    {
            
        var timePassed = 0f;
        while (timePassed < duration)
        {
            timePassed += Time.smoothDeltaTime;
            var newAlpha = Mathf.Lerp(start, end, timePassed / duration);
            foreach (var material in _materials)
                SetAlpha(material, newAlpha);
            yield return new WaitForFixedUpdate();
        }
    }

    private void SetAlpha(Material material, float newValue)
    {
        var oldColor = material.color;
        var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newValue);
        material.SetColor("_Color", newColor);
    }

    /*
    private void EnableSpriteRenders()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = true;
    }
    
    private void DisableSpriteRenderers()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
    }
    */
}