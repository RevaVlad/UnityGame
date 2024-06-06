using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MoveActionsAnimation : MonoBehaviour
{
    [SerializeField] private UnityEvent moveRightAction;
    [SerializeField] private UnityEvent moveLeftAction;
    [SerializeField] private UnityEvent moveStopped;

    private void OnMove(InputValue inputValue)
    {
        var direction = inputValue.Get<Vector2>();

        switch (direction.x)
        {
            case > 0:
                moveRightAction.Invoke();
                break;
            case < 0:
                moveLeftAction.Invoke();
                break;
            default:
                moveStopped.Invoke();
                break;
        }
    }
}
