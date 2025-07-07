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

        KillCountExponent = 0;
        BarePassExponent = 0;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnWall = true;
            GameSceneEventHandler.instance.EnterWall();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnWall = false;
            GameSceneEventHandler.instance.LeaveWall();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnInvisWall = true;
            GameSceneEventHandler.instance.TouchInvisibleWall();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnInvisWall = false;
            GameSceneEventHandler.instance.LeaveInvisibleWall();
        }
    }

    public void OnPlayerDie()
    {
        currentCollider.enabled = false;
    }

    public void PlayerTotalIgnoreCollision(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(6, 8, ignore);
        Physics2D.IgnoreLayerCollision(6, 9, ignore);
    }

    public void PlayerIgnoreCollisionEnemy(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(6, 8, ignore);

    }
    public void BarePass()
    {
        BarePassExponent = BarePassExponent >= playerData.MaxCombo ? playerData.MaxCombo : ++BarePassExponent;
        PlayerManager.instance.Point += 15 * BarePassExponent * PlayerManager.instance.playerPowerUps.PointExponent;

        GameSceneUIManager.instance.BarePassToText(BarePassExponent, PlayerManager.instance.playerPowerUps.PointExponent);
        PlayerManager.instance.playerMovement.BarePass();
    }

    public void EnemyKilled()
    {
        KillCountExponent = KillCountExponent >= playerData.MaxCombo ? playerData.MaxCombo : ++KillCountExponent;

        PlayerManager.instance.Point += (15 * KillCountExponent * PlayerManager.instance.playerPowerUps.PointExponent);
        playerData.Coins += (15 * KillCountExponent * PlayerManager.instance.playerPowerUps.PointExponent);

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
                currentCollider = CircleCollider;
                break;
            case PlayerShape.Triangle:
                currentCollider = CircleCollider;
                break;
        }

        currentCollider.enabled = true;
    }
}
