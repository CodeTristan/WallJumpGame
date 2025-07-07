using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObject : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.instance.playerData.Coins += amount;
            GameSceneUIManager.instance.CoinGainText();
            gameObject.SetActive(false);
        }
    }
}
