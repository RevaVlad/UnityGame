using UnityEngine;
using UnityEngine.U2D;

public class SpriteShapeAnimator : MonoBehaviour
{
    private SpriteShapeController _spriteShapeController;
    [SerializeField] private float animationSpeed = 1.0f;
    [SerializeField] private float amplitude = 1.0f;

    private Spline _spline;
    private Vector3[] _originalPositions;

    private void Start()
    {
        if (_spriteShapeController == null)
            _spriteShapeController = GetComponent<SpriteShapeController>();

        _spline = _spriteShapeController.spline;
        _originalPositions = new Vector3[_spline.GetPointCount()];

        for (var i = 0; i < _spline.GetPointCount(); i++) 
            _originalPositions[i] = _spline.GetPosition(i);
    }

    private void Update()
    {
        AnimatePoints();
    }

    private void AnimatePoints()
    {
        for (var i = 0; i < _spline.GetPointCount(); i++)
        {
            var originalPosition = _originalPositions[i];
            var offsetX = Mathf.Sin(Time.time * animationSpeed + i) * amplitude;
            var offsetY = Mathf.Cos(Time.time * animationSpeed + i) * amplitude;

            var newPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);
            _spline.SetPosition(i, newPosition);
        }

        _spriteShapeController.RefreshSpriteShape();
    }
}

