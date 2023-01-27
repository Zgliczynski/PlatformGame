using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Parameters, State & Components
    //Components
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    //State
    public bool IsFacingRight { get; private set; }

    //Check
    [Header("Check")]
    [SerializeField] private Transform wallChackPoint;
    [SerializeField] private Vector2 wallCheckPointSize;

    //Layer
    [Header("Layer & Tags")]
    [SerializeField] private LayerMask wallCheck;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float directionX;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        IsFacingRight = true;
        directionX = 1;
    }

    private void Update()
    {
        //Collision Check
        if (Physics2D.OverlapBox(wallChackPoint.position, wallCheckPointSize, 0, wallCheck) && IsFacingRight)
            directionX = -1;

        if (Physics2D.OverlapBox(wallChackPoint.position, wallCheckPointSize, 0, wallCheck) && !IsFacingRight)
            directionX = 1;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(directionX * moveSpeed, rb.velocity.y);
    }

    private void LateUpdate()
    {
        ChangeFaceAfterHitWall();
    }

    private void ChangeFaceAfterHitWall()
    {
        Vector3 scale = transform.localScale;

        if (rb.velocity.x > 0 && directionX == -1)
        {
            scale.x *= -1;
            transform.localScale = scale;
            directionX = -1;
        }
        else if(rb.velocity.x < 0 && directionX == 1)
        {
            scale.x *= 1;
            transform.localScale = scale;
            directionX = 1;
        }
        else if(rb.velocity.x == 0 && directionX == -1)
        {
            scale.x *= -1;
            transform.localScale = scale;
            directionX = 1;
        }
        else if (rb.velocity.x == 0 && directionX == 1)
        {
            scale.x *= -1;
            transform.localScale = scale;
            directionX = -1;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallChackPoint.position, wallCheckPointSize);
    }
}
