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

        PlayerEventHandler.instance.OnPlayerDamaged += TakeDamage;
        PlayerEventHandler.instance.OnPlayerDied += Die;
    }

    private void OnDestroy()
    {
        PlayerEventHandler.instance.OnPlayerDamaged -= TakeDamage;
        PlayerEventHandler.instance.OnPlayerDied -= Die;
    }

    public void TakeDamage()
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            PlayerEventHandler.instance.PlayerDied();
        }
    }

    private void Die()
    {
        ////Add Animation
        Debug.Log("Player Died");
        PlayerManager.instance.isDead = true;
    }
}
