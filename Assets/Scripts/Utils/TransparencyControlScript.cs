using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TransparencyControlScript : MonoBehaviour
{
    private Coroutine _currentAnimation = null;
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

    public void Appear()
    {
        if (_currentAnimation is not null) StopCoroutine(_currentAnimation);
        StartCoroutine(InterpolateTransparencyCoroutine(1, appearTime));
    }

    public void Disappear()
    {
        if (_currentAnimation is not null) StopCoroutine(_currentAnimation);
        StartCoroutine(InterpolateTransparencyCoroutine(0, disappearTime));
    }

    private IEnumerator InterpolateTransparencyCoroutine(float end, float duration)
    {
        var start = _materials.First().color.a;
        var timePassed = 0f;
        while (timePassed < duration)
        {
            timePassed += Time.smoothDeltaTime;
            var newAlpha = Mathf.Lerp(start, end, timePassed / duration);
            foreach (var material in _materials)
                SetAlpha(material, newAlpha);
            yield return new WaitForFixedUpdate();
        }

        _currentAnimation = null;
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