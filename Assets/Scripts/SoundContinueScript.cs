using UnityEngine;

public class SoundContinueScript : MonoBehaviour
{
    private static SoundContinueScript _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}