using UnityEngine;
using UnityEngine.InputSystem;

public class HeroConnectWithLadders : MonoBehaviour
{
    private GameObject _player;
    private bool _isPlayerConnectedToThisLadder = false;
    private PlayerControls _laddersControls;

    // Start is called before the first frame update
    void Start()
    {
        _laddersControls = new PlayerControls();
    }

    private void OnMergePlayerandLadder()
    {
        CheckCollisionWithLadder();
        if (!_player) return;
        if (!_isPlayerConnectedToThisLadder)
        {
            _player.transform.SetParent(transform);
            _isPlayerConnectedToThisLadder = true;
            _player.transform.GetComponent<HeroScript>().isPlayerOnLadder = true;
        }
        else
        {
            _player.transform.SetParent(null);
            _isPlayerConnectedToThisLadder = false;
            _player.transform.GetComponent<HeroScript>().isPlayerOnLadder = false;
            _player = null;
        }
    }

    private void OnMoveLadder(InputValue inputValue)
    {
        if (_isPlayerConnectedToThisLadder)
        {
            var direction = (int)inputValue.Get<float>();
            if (direction > 0)
                transform.GetComponent<LadderScript>().MoveRight();
            if (direction < 0)
                transform.GetComponent<LadderScript>().MoveLeft();
        }
    }

    private void CheckCollisionWithLadder()
    {
        var collider = Physics2D.OverlapCircleAll(transform.position, 0.25f, LayerMask.GetMask("Player"));
        if (collider.Length == 0)
        {
            _player = null;
            return;
        }

        _player = collider[0].gameObject;
    }
}