using UnityEngine;
using UnityEngine.InputSystem;

public class HeroConnectWithLadders : MonoBehaviour
{
    private GameObject _player;
    private bool _isPlayerWithLadder = false;
    private PlayerControls _laddersControls;
    
    // Start is called before the first frame update
    void Start()
    {
        _laddersControls = new PlayerControls();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
    }

    private void OnMergePlayerandLadder()
    {
        if (!_player) return;
        if (!_isPlayerWithLadder)
        {
            _player.transform.SetParent(this.transform);
            _isPlayerWithLadder = true;
            _player.transform.GetComponent<HeroScript>().isPlayerOnLadder = true;
        }
        else
        {
            _player.transform.SetParent(null);
            _isPlayerWithLadder = false;
            _player.transform.GetComponent<HeroScript>().isPlayerOnLadder = false;
        }
    }

    private void OnMoveLadder(InputValue inputValue)
    {
        if (_isPlayerWithLadder)
        {
            var direction = (int)inputValue.Get<float>();
            if (direction > 0)
                transform.GetComponent<LadderScript>().MoveRight();
            if (direction < 0)
                transform.GetComponent<LadderScript>().MoveLeft();
        }
    }

    private void CheckCollision()
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
