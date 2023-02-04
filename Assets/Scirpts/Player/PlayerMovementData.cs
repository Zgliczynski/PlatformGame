using UnityEngine;

[CreateAssetMenu(menuName = "Player Movement Data")]
public class PlayerMovementData : ScriptableObject
{
    [Header("Run")]
    public float runMaxSpeed = 20; //Target speed.
    public float runAcceleration = 2; //Time from 0 speed to max speed.
    [HideInInspector] public float runAccelAmount; //Actual force to acceleration player.
    public float runDecceleration = 1.5f; //Time from max speed to 0.
    [HideInInspector] public float runDeccelAmount; //Actual force to decceleretion player.
    [Space(10)]
    [Range(0.01f, 1)] public float accelInAir; //Acceleration in free air.
    [Range(0.01f, 1)] public float deccelInAir; //Decceleration in free air.
    public bool doConserveMomentum;

    [Header("Gravity")]
    [HideInInspector] public float gravityStrength; //Downwards force needed for jumpHight & jumpTimeToApex.
    [HideInInspector] public float gravityScale; //Strangth of the player's gravity as a multiplier of gravity.
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed; //Maximum fall speed 
    public float fastFallGravityMult; //Faster fall when input the down button
    public float maxFastFallSpeed; //Max fall speed

    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height.
    [HideInInspector] public float jumpForce; //The actual force applied to the player jump.
    public float jumpAfterEnemyDie;

    [Header("Both Jump")]
    public float jumpCutGravityMult; //Multiplier to increase gravity if button "jump" still down.
    public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will expirience extra "jump hang", velocity.y close to 0 at the apex.
    [Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex(maxHeight).
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
    [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
    public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction

    [Header("Slide")]
    public float slideSpeed;
    public float slideAccel;

    [Header("Health")]
    public int playerHealth;

    [Header("Assists System")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //When you falling off the platform, can still jump.
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Can faster jump if you sill in air.

    private void OnValidate()
    {
        //Calculate run acceleration & decceleration forces  formula: ((1 / Time.fixedDeltaTime) * acceleration / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion

        //Calculate gravity strenght using formula: (gravity = 2 * jumpHeight / jumpTimeToApex^2)
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Calculate the rigidbody's gravity scale
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Calculate jumpForce using formula: (initialJumpVelocity = gravity * jumpTimeToApex)
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
    }
}
