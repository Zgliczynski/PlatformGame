using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerMovementData Data;
    #region Parameters, State & Components
    //Components
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    //State parameters
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsSliding { get; private set; }
    public bool IsWallJumping { get; private set; }
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }
    public float LastPressedJumpTime { get; private set; }

    //Input parameters
    private Vector2 moveInput;

    //Check parameters
    [Header("Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.41f, 0.262f);
    [SerializeField] private Transform _enemyHeadCheck;
    [SerializeField] private Vector2 _enemyHeadCheckSize = new Vector2(0f, 0f);

    //Layers & tags
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;

    //Jump Parameters
    private bool isJumpCut;
    private bool isJumpFalling;

    //Wall Jump Parameters
    private float wallJumpStartTime;
    private int lastWallDirectionJump;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

    }

    private void Start()
    {
        IsFacingRight = true;
        SetGravityScale(Data.gravityScale);
    }

    private void Update()
    {
        //Timer
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;

        //Input Handler
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.x != 0)
            CheckDirectionToFace(moveInput.x > 0);

        if (Input.GetKeyDown(KeyCode.Space))
            OnJumpInput();

        if (Input.GetKeyDown(KeyCode.Space))
            OnJumpUpInput();

        #region Collision Check
        //Ground Check
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping)
            LastOnGroundTime = Data.coyoteTime; //sets the lastGrounded to coyoteTime.

        //Right Wall Check
        if ((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
            || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight) && !IsWallJumping)
            LastOnWallRightTime = Data.coyoteTime;

        //Left Wall Check
        if ((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
            || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight) && !IsWallJumping)
            LastOnWallLeftTime = Data.coyoteTime;

        //Double check for both side
        LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        #endregion

        #region Jump Check
        if (IsJumping && rb.velocity.y < 0)
        {
            IsJumping = false;
            if (!IsWallJumping)
                isJumpFalling = true;
        }

        if(IsWallJumping && Time.time - wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if(LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            isJumpCut = false;
            if (!IsJumping)
                isJumpFalling = false;
        }

        //Jump
        if(CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            IsWallJumping = false;
            isJumpCut = false;
            isJumpFalling = false;
            Jump();
        }//Wall Jump
        else if(CanWallJump() && LastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            isJumpCut = false;
            isJumpFalling = false;
            wallJumpStartTime = Time.time;
            lastWallDirectionJump = (LastOnWallRightTime > 0) ? -1 : 1;

            WallJump(lastWallDirectionJump);
        }
        #endregion
        
        #region Slide Check
        if (CanSlide() && (LastOnWallLeftTime > 0 && moveInput.x < 0) || (LastOnWallRightTime > 0 && moveInput.x > 0))
            IsSliding = true;
        else
            IsSliding = false;
        #endregion

        #region Gravity Check
        //Higher gravity if we have relesed the jump input or falling
        if (IsSliding)
        {
            SetGravityScale(0);
        }
        else if(rb.velocity.y < 0 && moveInput.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fastFallGravityMult); //Much higher gravity if button down.
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
        }
        else if (isJumpCut)
        {
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult); //Higher gravity if jump button released.
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
        }
        else if((IsJumping || IsWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        else if(rb.velocity.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fallGravityMult); //Higher gravity when falling.
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
        }
        else
        {
            SetGravityScale(Data.gravityScale); //Default gravity
        }
        #endregion
    }

    private void FixedUpdate()
    {
        //Handle Run
        if (IsWallJumping)
            Run(Data.wallJumpRunLerp);
        else
            Run(1);

        //Handle slide
        if (IsSliding)
            Slide();
    }

    #region Damage

    private void TakeDamage()
    {
        Data.playerHealth--;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string name = collision.gameObject.name;
        if(name == "Enemy")
        {
            TakeDamage();
        }

    }

    #endregion

    #region Input Callback
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            isJumpCut = true;
    }
    #endregion

    #region General Gravity Methods
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }
    #endregion
    
    #region Run
    private void Run(float lerpAmount)
    {
        float targetSpeed = moveInput.x * Data.runMaxSpeed;

        //Calculate AccelRate
        float accelRate;

        //gets an acceleration value based on if we are acceleration,
        //or trying to decelerate (stop), as well applying a mutiplier of we are in air.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;

        if((IsJumping || IsWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        //we won't slow the player down if they are moving in their desired direction but at greater speed than their maxSpeed
        if(Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == 
            Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        float speedDif = targetSpeed - rb.velocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if(tag == "PlayerBuffJump")
        {
            rb.AddForce(transform.up * Data.jumpAfterEnemyDie, ForceMode2D.Impulse);
        }
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }

    #region Jump
    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        //Perform Jump
        float force = Data.jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        //Perform Wall Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //Apply force in diff side of wall

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0)
            force.y -= rb.velocity.y;

        rb.AddForce(force, ForceMode2D.Impulse);
    }
    #endregion

    #region Slide
    private void Slide()
    {
        float speedDif = Data.slideSpeed - rb.velocity.y;
        float movement = speedDif * Data.slideAccel;

        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }
    #endregion

    #region Check Methods
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
            (LastOnWallRightTime > 0 && lastWallDirectionJump == 1) || (LastOnWallLeftTime > 0 && lastWallDirectionJump == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && rb.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && rb.velocity.y > 0;
    }

    private bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion

    #region Editor Methods
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_enemyHeadCheck.position, _enemyHeadCheckSize);
    }
    #endregion
}

