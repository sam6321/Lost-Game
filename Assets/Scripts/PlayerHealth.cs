using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int health = 5;

    [SerializeField]
    private GameObject gameOverDisplay;

    //private HealthRemainingUI healthRemaining;
    private PlayerMovement movement;
    private FadeManager fadeManager;
    private BoardCreator boardCreator;
    private RemainingUI healthRemaining;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        GameObject gameManager = GameObject.Find("GameManager");
        fadeManager = gameManager.GetComponent<FadeManager>();
        boardCreator = gameManager.GetComponent<BoardCreator>();
        healthRemaining = GameObject.Find("HealthRemaining").GetComponent<RemainingUI>();
        ResetHealth();
    }

    public void ResetHealth()
    {
        SetHealth(5);
    }

    public void SetHealth(int amount)
    {
        health = amount;
        healthRemaining.SetCount(health);
        if (health <= 0)
        {
            // Die
            movement.enabled = false;
            StartCoroutine(WaitAndShowDeath());
        }
        else
        {
            movement.enabled = true;
        }
    }

    public void ModifyHealth(int amount)
    {
        SetHealth(health + amount);
    }

    IEnumerator WaitAndShowDeath()
    {
        yield return new WaitForSeconds(0.5f);
        fadeManager.StartFade(() => {
            gameOverDisplay.SetActive(true);
            boardCreator.DestroyLevel();
        });
    }

    void OnTakeDamage(DamageInfo info)
    {
        Debug.Log("Player Hit for " + info.damage);
        ModifyHealth(-info.damage);
    }
}
