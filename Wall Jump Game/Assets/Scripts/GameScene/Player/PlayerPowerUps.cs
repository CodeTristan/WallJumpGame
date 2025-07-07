using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public int PointExponent = 1;

    private float currentBomberTimer;
    private float currentDoublePointTimer;

    public List<PowerUpType> currentPowerUps = new List<PowerUpType>();

    private PlayerMovement PlayerMovement;
    private PlayerData PlayerData;
    public void Init()
    {
        PlayerMovement = PlayerManager.instance.playerMovement;
        PlayerData = PlayerManager.instance.playerData;
        currentPowerUps.Clear();

    }

    private void Update()
    {
        if (AdManager.instance.InAdMenu) //WAIT IN AD MENU
            return;

        if (currentPowerUps.Contains(PowerUpType.DoublePoint))
        {
            currentDoublePointTimer -= Time.deltaTime;
            GameSceneUIManager.instance.UpdateDoublePointText(currentDoublePointTimer);
            if (currentDoublePointTimer <= 0)
            {
                GameSceneUIManager.instance.ToggleDoublePointText(false);
                currentPowerUps.Remove(PowerUpType.DoublePoint);
                PointExponent = 1;
            }
        }

        if (currentPowerUps.Contains(PowerUpType.Bomber))
        {
            currentBomberTimer -= Time.deltaTime;
            GameSceneUIManager.instance.UpdateBomberText(currentBomberTimer);
            if (currentBomberTimer <= 0)
            {
                GameSceneUIManager.instance.ToggleBomberText(false);
                currentPowerUps.Remove(PowerUpType.Bomber);
            }
        }
    }

    public bool HasPowerUp(PowerUpType powerUp)
    {
        return currentPowerUps.Contains(powerUp);
    }

    public void AddPowerUp(PowerUpType powerUp)
    {
        if (currentPowerUps.Contains(powerUp))
        {
            switch (powerUp)
            {
                case PowerUpType.DoublePoint:
                    currentDoublePointTimer = PlayerData.PowerUpData.DoublePointTimer;
                    PointExponent++;
                    break;
                case PowerUpType.Bomber:
                    currentBomberTimer = PlayerData.PowerUpData.BomberTimer;
                    break;
            }
        }
        else
        {
            currentPowerUps.Add(powerUp);
            switch (powerUp)
            {
                case PowerUpType.DoublePoint:
                    GameSceneUIManager.instance.ToggleDoublePointText(true);
                    currentDoublePointTimer = PlayerData.PowerUpData.DoublePointTimer;
                    PointExponent = 2;
                    break;
                case PowerUpType.Bomber:
                    GameSceneUIManager.instance.ToggleBomberText(true);
                    currentBomberTimer = PlayerData.PowerUpData.BomberTimer;
                    break;
                case PowerUpType.SlingShot:
                    StopAllCoroutines();
                    StartCoroutine(SlingShot());
                    break;
            }
        }
    }

    private IEnumerator SlingShot()
    {
        PlayerMovement.ResetValues();
        PlayerMovement.ResetVelocity();

        PlayerCollisionHandler playerCollisionHandler = PlayerManager.instance.playerCollisionHandler;
        playerCollisionHandler.PlayerIgnoreCollisionEnemy(true);
        animator.SetBool("isInvis", true);

        PlayerMovement.rb.AddForce(Vector2.up * PlayerData.PowerUpData.SlingShotPower, ForceMode2D.Impulse);

        while (PlayerMovement.rb.velocity.y > 1)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        playerCollisionHandler.PlayerIgnoreCollisionEnemy(false);

        animator.SetBool("isInvis", false);

    }
}
