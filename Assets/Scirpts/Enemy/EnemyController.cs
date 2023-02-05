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
    public bool IsKill { get; private set; }

    //Check
    [Header("Check")]
    [SerializeField] private Transform wallChackPoint;
    [SerializeField] private Vector2 wallCheckPointSize;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private Vector2 playerCheckSize;

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
        directionX = 1; //Checking direction on start game always right.
    }

    private void Update()
    {
        Kill(false);
         
        //Collision Check
        //Checking collision with wall on right side.
        if (Physics2D.OverlapBox(wallChackPoint.position, wallCheckPointSize, 0, wallCheck) && IsFacingRight)
            directionX = -1;

        //Checking collision with wall on left side.
        if (Physics2D.OverlapBox(wallChackPoint.position, wallCheckPointSize, 0, wallCheck) && !IsFacingRight)
            directionX = 1;
    }

    private void FixedUpdate()
    {
        //Enemy move.
        rb.velocity = new Vector2(directionX * moveSpeed, rb.velocity.y);
    }

    private void LateUpdate()
    {
        ChangeFaceAfterHitWall();
    }

    private void ChangeFaceAfterHitWall()
    {
        Vector3 scale = transform.localScale;

        if(rb.velocity.x == 0 && directionX == -1)
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

    public void Kill(bool isKill)
    {
        isKill = IsKill;

        //Rotation after dead.
        Quaternion deadStartPosition = transform.rotation;
        Quaternion deadEndPosition = Quaternion.Euler(90, 0, 90);
        float timer = Time.deltaTime;

        if (isKill == true)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            transform.rotation = Quaternion.Lerp(deadStartPosition, deadEndPosition, timer);
            StartCoroutine(Die());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "EnemyHeadCheck")
        {
            IsKill = true;
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallChackPoint.position, wallCheckPointSize);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(playerCheck.position, playerCheckSize);
    }
}
