using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static PlayerSprite;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private CircleCollider2D CircleCollider;
    [SerializeField] private BoxCollider2D CubeCollider;
    [SerializeField] private PolygonCollider2D TriangleCollider;

    [HideInInspector] public Collider2D currentCollider;

    public int KillCountExponent;
    public int BarePassExponent;

    private PlayerData playerData;

    public void Init()
    {
        currentCollider = CircleCollider;
        playerData = PlayerManager.instance.playerData;

        PlayerEventHandler.instance.OnEnemyKilled += EnemyKilled;
        PlayerEventHandler.instance.OnPlayerDied += OnPlayerDied;
    }

    private void OnDestroy()
    {
        PlayerEventHandler.instance.OnEnemyKilled -= EnemyKilled;
        PlayerEventHandler.instance.OnPlayerDied -= OnPlayerDied;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnWall = true;
            PlayerEventHandler.instance.TouchWall();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnWall = false;
            PlayerEventHandler.instance.LeaveWall();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnInvisWall = true;
            PlayerEventHandler.instance.TouchInvisibleWall();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnInvisWall = false;
            PlayerEventHandler.instance.LeaveInvisibleWall();
        }
    }

    private void OnPlayerDied()
    {
        currentCollider.enabled = false;
    }

    public void BarePass()
    {
        BarePassExponent = BarePassExponent > playerData.MaxCombo ? playerData.MaxCombo : ++BarePassExponent;
        PlayerManager.instance.Point += 15 * BarePassExponent * PlayerManager.instance.playerPowerUps.PointExponent;

        GameSceneUIManager.instance.BarePassToText(BarePassExponent, PlayerManager.instance.playerPowerUps.PointExponent);
    }

    private void EnemyKilled()
    {
        KillCountExponent = KillCountExponent > playerData.MaxCombo ? playerData.MaxCombo : ++KillCountExponent;

        PlayerManager.instance.Point += (15 * KillCountExponent * PlayerManager.instance.playerPowerUps.PointExponent);
        playerData.Coin += (15 * KillCountExponent * PlayerManager.instance.playerPowerUps.PointExponent);

        GameSceneUIManager.instance.EnemyKilledToText(KillCountExponent, PlayerManager.instance.playerPowerUps.PointExponent);
    }

    public void ChangeCollider(PlayerShape shape)
    {
        currentCollider.enabled = false;

        switch(shape)
        {
            case PlayerShape.Sphere: 
                currentCollider = CircleCollider;
                break;
            case PlayerShape.Cube:
                currentCollider = CubeCollider;
                break;
            case PlayerShape.Triangle:
                currentCollider = TriangleCollider;
                break;
        }

        currentCollider.enabled = true;
    }
}
