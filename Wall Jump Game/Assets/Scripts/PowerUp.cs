using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PowerUpType
{
    DoublePoint,
    Bomber,
    SlingShot
}
public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerManager.instance.playerPowerUps.AddPowerUp(powerUpType);
            Destroy(gameObject);
        }
    }
}
