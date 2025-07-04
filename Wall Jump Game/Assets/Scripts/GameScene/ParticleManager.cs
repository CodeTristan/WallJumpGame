using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    [SerializeField] private ParticleSystem bouncyWallParticle;
    [SerializeField] private ParticleSystem enemyDeathParticle;

    private Dictionary<ParticleType, ParticleSystem> particleDict;
    public enum ParticleType
    {
        BouncyWall,
        EnemyDie
    }

    public void Init()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        particleDict = new Dictionary<ParticleType, ParticleSystem>();
        particleDict.Add(ParticleType.BouncyWall, bouncyWallParticle);
        particleDict.Add(ParticleType.EnemyDie, enemyDeathParticle);
    }

    public void PlayParticle(ParticleType type ,Vector2 position)
    {
        if (particleDict.ContainsKey(type) == false)
        {
            Debug.LogError("Particle type not found: " + type);
            return;
        }
        ParticleSystem particle = particleDict[type];

        if (particle.isPlaying == false)
        {
            particle.transform.position = position;
            particle.Play();
        }
        else
        {
            particle.Stop();
            particle.transform.position = position;
            particle.Play();
        }

    }
}
