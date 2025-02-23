using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static PlayerSprite;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] LayerMask WallLayer;
    [SerializeField] private CircleCollider2D CircleCollider;
    [SerializeField] private BoxCollider2D CubeCollider;
    [SerializeField] private PolygonCollider2D TriangleCollider;

    [HideInInspector] public Collider2D currentCollider;

    private float KillCountTimer = 1.5f;
    private float currentKillCountTimer;
    private int KillCountExponent;

    private PlayerData playerData;

    public void Init()
    {
        currentCollider = CircleCollider;
        playerData = PlayerManager.instance.playerData;

        PlayerEventHandler.OnEnemyKilled += EnemyKilled;
    }

    private void OnDestroy()
    {
        PlayerEventHandler.OnEnemyKilled -= EnemyKilled;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == WallLayer)
        {
            PlayerManager.instance.OnWall = true;
            PlayerEventHandler.TouchWall();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == WallLayer)
        {
            PlayerManager.instance.OnWall = false;
            PlayerEventHandler.LeaveWall();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == WallLayer)
        {
            PlayerManager.instance.OnInvisWall = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == WallLayer)
        {
            PlayerManager.instance.OnInvisWall = false;
        }
    }

    private void EnemyKilled()
    {
        KillCountExponent = KillCountExponent > playerData.MaxCombo ? playerData.MaxCombo : ++KillCountExponent;
        currentKillCountTimer = KillCountTimer;

        PlayerManager.instance.Point += (15 * KillCountExponent * PlayerManager.instance.playerPowerUps.PointExponent);
        playerData.Coin += (15 * KillCountExponent * PlayerManager.instance.playerPowerUps.PointExponent);

        GameSceneUIManager.instance.EnemyKilledToText(KillCountExponent, PlayerManager.instance.playerPowerUps.PointExponent);
    }


}
