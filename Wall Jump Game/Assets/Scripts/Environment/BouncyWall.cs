using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyWall : MonoBehaviour
{
    public Vector3 dir;
    public float power;

    private void Start()
    {
        dir = new Vector3(dir.x * power, dir.y * power, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Force);
            ParticleManager.instance.PlayParticle(ParticleManager.ParticleType.BouncyWall,collision.transform.position);
        }
    }
}
