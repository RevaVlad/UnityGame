using UnityEngine;

[CreateAssetMenu(menuName = "Player Run Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class PlayerRunData : ScriptableObject
{
    [Header("Run")]
    public float runMaxSpeed; // Target speed we want the player to reach.

    public float animationSpeed;
    public float frictionAmount;
    public float acceleration; // The actual force (multiplied with speedDiff) applied to the player.
    public float decceleration; //Actual force (multiplied with speedDiff) applied to the player .
    
    [Space(10)]
    [Range(0.01f, 1)] public float airAccelerationRation; //Multipliers applied to acceleration rate when airborne.
    [Range(0.01f, 1)] public float airDeccelerationRation;
    [HideInInspector] public float airAcceleration;
    [HideInInspector] public float airDecceleration;
    public bool doConserveMomentum;

    private void OnValidate()
    {
        airAcceleration = acceleration * airAccelerationRation;
        airDecceleration = decceleration * airDeccelerationRation;
    }
}