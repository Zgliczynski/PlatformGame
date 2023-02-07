using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHearts : MonoBehaviour
{
    public PlayerMovementData Data;
    private CircleCollider2D circleCollider2D;
    private Rigidbody2D rb;
    [SerializeField] private int value;

    private void Awake()
    {

        circleCollider2D = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        HeartsUIController _health = collision.GetComponent<HeartsUIController>();
        if (tag == "MyPlayer" && _health.numberOfHealth == Data.playerHealth)
        {
            _health.numberOfHealth++;
            Destroy(gameObject);
            Debug.Log("first");
        }
        else if (tag == "MyPlayer" && _health.numberOfHealth > Data.playerHealth)
        {
            Data.playerHealth++;
            Destroy(gameObject);
            Debug.Log("secend");
        }
    }
}
               
