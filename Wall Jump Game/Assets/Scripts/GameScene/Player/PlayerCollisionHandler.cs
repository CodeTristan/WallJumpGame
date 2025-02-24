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

        PlayerEventHandler.OnEnemyKilled += EnemyKilled;
    }

    private void OnDestroy()
    {
        PlayerEventHandler.OnEnemyKilled -= EnemyKilled;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnWall = true;
            PlayerEventHandler.TouchWall();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerManager.instance.OnWall = false;
            PlayerEventHandler.LeaveWall();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("Trigger Enter: " + collision.name);
            PlayerManager.instance.OnInvisWall = true;
            PlayerEventHandler.TouchInvisibleWall();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("Trigger Enter: " + collision.name);
            PlayerManager.instance.OnInvisWall = false;
            PlayerEventHandler.LeaveInvisibleWall();
        }
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
