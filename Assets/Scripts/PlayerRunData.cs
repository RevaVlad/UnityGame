using UnityEngine;

[CreateAssetMenu(menuName = "Player Run Data")]
public class PlayerRunData : ScriptableObject
{
    [Header("Run")] public float runMaxSpeed;

    public float animationSpeed;
    public float frictionAmount;
    public float acceleration;
    public float decceleration;

    [Space(10)] public float airAccelerationRation;
    public float airDeccelerationRation;
    [HideInInspector] public float airAcceleration;
    [HideInInspector] public float airDecceleration;

    [Header("Jump")] public float jumpForce;
    public float coyoteTime;
    public float bufferTime;
    public float jumpCutMultiplier;
    public float momentumLossAtJump;
    public float gravityScaleWhenFalling;
    public float normalGravityScale;

    private void OnValidate()
    {
        airAcceleration = acceleration * airAccelerationRation;
        airDecceleration = decceleration * airDeccelerationRation;
    }
}