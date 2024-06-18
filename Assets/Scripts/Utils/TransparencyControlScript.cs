using System.Collections;
using System.Linq;
using UnityEngine;

public class TransparencyControlScript : MonoBehaviour
{
    private Coroutine currentAnimation;
    [SerializeField] private bool startTransparent;

    private Material[] materials;
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    private void Start()
    {
        materials = GetComponentsInChildren<SpriteRenderer>().Select(rend => rend.material).ToArray();

        if (!startTransparent) return;
        foreach (var material in materials)
            SetAlpha(material, 0);
    }

    public void Appear()
    {
        if (!gameObject.activeInHierarchy) return;
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(InterpolateTransparencyCoroutine(1, Utils.TransparencyAppearTime));
    }

    public void Disappear()
    {
        if (!gameObject.activeInHierarchy) return;
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(InterpolateTransparencyCoroutine(0, Utils.TransparencyDisappearTime));
    }

    private IEnumerator InterpolateTransparencyCoroutine(float end, float duration)
    {
        var start = materials.First().color.a;
        var timePassed = 0f;
        while (timePassed < duration)
        {
            timePassed += Time.smoothDeltaTime;
            var newAlpha = Mathf.Lerp(start, end, timePassed / duration);
            foreach (var material in materials)
                SetAlpha(material, newAlpha);
            yield return new WaitForFixedUpdate();
        }

        currentAnimation = null;
    }

    private static void SetAlpha(Material material, float newValue)
    {
        var oldColor = material.color;
        var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newValue);
        material.SetColor(Color1, newColor);
    }
}