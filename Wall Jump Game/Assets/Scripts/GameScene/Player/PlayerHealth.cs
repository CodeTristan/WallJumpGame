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

        PlayerEventHandler.OnPlayerDamaged += TakeDamage;
        PlayerEventHandler.OnPlayerDied += Die;
    }

    private void OnDestroy()
    {
        PlayerEventHandler.OnPlayerDamaged -= TakeDamage;
        PlayerEventHandler.OnPlayerDied -= Die;
    }

    public void TakeDamage()
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            PlayerEventHandler.PlayerDied();
        }
    }

    private void Die()
    {
        ////Add Animation
        Debug.Log("Player Died");
        PlayerManager.instance.isDead = true;
        //GainedCoinText.gameObject.GetComponent<Animator>().SetBool("Dead", true);
        //Coin += Point * 3 / 10;

        //if (PlayerPrefs.GetInt("MaxPoint") < Point)
        //    PlayerPrefs.SetInt("MaxPoint", Point);

        //DeathMaxPointText.text = PlayerPrefs.GetInt("MaxPoint").ToString();
        //DeathPointText.text = Point.ToString();
        //GainedCoinText.text = "+ " + (Coin - currentCoin).ToString() + "$";

        //currentHealth = 0;
        //barePassImage.SetActive(false);
        //restartMenu.SetActive(true);
        //pointText.gameObject.SetActive(false);
        //rb.gravityScale = 0;
        //rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
