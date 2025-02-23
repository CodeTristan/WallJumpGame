using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerTouch();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerTouch();
        }
    }

    public virtual void OnPlayerTouch()
    {
        if(PlayerManager.instance.playerPowerUps.HasPowerUp(PowerUpType.Bomber))
        {
            Die();
            PlayerEventHandler.EnemyKilled();
            return;
        }

        PlayerEventHandler.PlayerDamaged();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

}
