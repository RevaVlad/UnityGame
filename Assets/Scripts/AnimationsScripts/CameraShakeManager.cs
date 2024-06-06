using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    [SerializeField] private float globalForce = 1f;

    public void CameraShake(CinemachineImpulseSource impulseSource) =>
        impulseSource.GenerateImpulseWithForce(globalForce);
}