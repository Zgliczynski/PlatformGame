using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUIController : MonoBehaviour
{
    private PlayerController playerController;
    public PlayerMovementData Data;

    [Header("Health")]
    public int health;
    public int numberOfHealth;

    [Header("Sprite/Image")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        health = Data.playerHealth;
        HeartController();
    }

    public void HeartController()
    {
        if (health > numberOfHealth)
        {
            health = numberOfHealth;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numberOfHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
