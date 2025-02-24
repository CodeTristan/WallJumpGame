using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeChanger : MonoBehaviour
{
    public PlayerSprite.PlayerShape shape;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.instance.playerSprite.ChangeShape(shape);
            Destroy(gameObject);
        }
    }
}
