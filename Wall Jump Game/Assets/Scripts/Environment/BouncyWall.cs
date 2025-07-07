using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyWall : MonoBehaviour
{
    public Vector2 dir;
    public float power;

    private void Start()
    {
        dir = new Vector3(dir.x * power, dir.y * power, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            Bounce(collision);
        }
    }


    private void Bounce(Collision2D collision)
    {

        PlayerManager.instance.playerMovement.rb.AddForce(dir, ForceMode2D.Force);
        ParticleManager.instance.PlayParticle(ParticleManager.ParticleType.BouncyWall, collision.transform.position);
    }

    private void HoneyWall(Collision2D collision)
    {
        PlayerManager.instance.playerMovement.ResetValues();

        Rigidbody2D rb = PlayerManager.instance.playerMovement.rb;
        rb.velocity = new Vector2(PlayerManager.instance.playerMovement.velocity.x * Mathf.Sign(dir.x), rb.velocity.y + dir.y);

        ParticleManager.instance.PlayParticle(ParticleManager.ParticleType.BouncyWall, collision.transform.position);
    }
}
