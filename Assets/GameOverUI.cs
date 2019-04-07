using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    private BoardCreator creator;
    private PlayerHealth playerHealth;

    void Start()
    {
        creator = GameObject.Find("GameManager").GetComponent<BoardCreator>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    public void OnYeClick()
    {
        creator.CreateLevel(true);
        playerHealth.ResetHealth();
        gameObject.SetActive(false);
    }

    public void OnNaClick()
    {
        Application.Quit();
    }
}
