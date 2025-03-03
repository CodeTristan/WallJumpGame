using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 1;
    public int currentHealth;


    public void Init()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage()
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            GameSceneEventHandler.instance.PlayerDied();
        }
    }

    public void Die()
    {
        ////Add Animation
        Debug.Log("Player Died");
        PlayerManager.instance.isDead = true;
    }
}
