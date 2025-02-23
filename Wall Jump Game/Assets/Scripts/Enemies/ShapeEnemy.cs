using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeEnemy : EnemyBase
{
    public PlayerSprite.PlayerShape shape;
    public float speed;

    public Vector2[] path;

    private Vector2 MoveTarget;

    [SerializeField] private int index;

    private void Start()
    {
        MoveTarget = path[0];
        MoveTarget = (Vector2)transform.position + MoveTarget;
    }

    private void Update()
    {
        if (Vector3.Distance(MoveTarget,transform.position) > 0.01)
            transform.position = Vector3.MoveTowards(transform.position, MoveTarget, speed * Time.deltaTime);
        else
        {
            index++;
            if(index >= path.Length)
            {
                index = 0;
            }
            MoveTarget = path[index];
            MoveTarget = (Vector2)transform.position + MoveTarget;
        }
    }

    public override void OnPlayerTouch()
    {
        if (PlayerManager.instance.playerPowerUps.HasPowerUp(PlayerPowerUps.powerUpType.Bomber)
            || PlayerManager.instance.playerSprite.currentShape == shape)
        {
            Die();
            PlayerEventHandler.EnemyKilled();
            return;
        }

        PlayerEventHandler.PlayerDamaged();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 startPos = transform.position;
        for (int i = 0; i < path.Length; i++)
        {
            Gizmos.DrawLine(startPos, startPos + path[(i) % path.Length]);
            startPos = startPos + path[i];
        }
    }
}
