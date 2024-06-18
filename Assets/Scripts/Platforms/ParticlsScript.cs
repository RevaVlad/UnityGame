using UnityEngine;

public class ParticlsScript : MonoBehaviour
{
    [SerializeField] private GameObject _object;

    public void SpawnParticls()
    {
        Instantiate(_object, transform.position, Quaternion.identity);
    }
}
