using UnityEngine;
using UnityEngine.U2D;

public class SpriteShapeAnimator : MonoBehaviour
{
    public SpriteShapeController spriteShapeController;
    public float animationSpeed = 1.0f;
    public float amplitude = 1.0f;

    private Spline spline;
    private Vector3[] originalPositions;

    void Start()
    {
        if (spriteShapeController == null)
        {
            spriteShapeController = GetComponent<SpriteShapeController>();
        }

        spline = spriteShapeController.spline;
        originalPositions = new Vector3[spline.GetPointCount()];

        for (var i = 0; i < spline.GetPointCount(); i++)
        {
            originalPositions[i] = spline.GetPosition(i);
        }
    }

    void Update()
    {
        AnimatePoints();
    }

    void AnimatePoints()
    {
        for (var i = 0; i < spline.GetPointCount(); i++)
        {
            var originalPosition = originalPositions[i];
            var offsetX = Mathf.Sin(Time.time * animationSpeed + i) * amplitude;
            var offsetY = Mathf.Cos(Time.time * animationSpeed + i) * amplitude;

            var newPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);
            spline.SetPosition(i, newPosition);
        }

        spriteShapeController.RefreshSpriteShape();
    }
}

