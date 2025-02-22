using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 1;
    public int currentHealth;

    private bool isDead;

    private void Init()
    {
        currentHealth = maxHealth;
    }
}
