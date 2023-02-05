using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHearts : MonoBehaviour
{
    private Health health;
    public PlayerMovementData Data;
    private CircleCollider2D circleCollider2D;
    private Rigidbody2D rb;
    [SerializeField] private int value;

    private void Awake()
    {
        health = GetComponent<Health>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if(tag == "AddHearts")
        {
            health.AddHeart();
            Destroy(gameObject);
        }
    }
}
