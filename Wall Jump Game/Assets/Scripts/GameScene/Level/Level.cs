using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelType Type;
    public Vector2 SpawnOffset = new Vector2(0, 30);

    public GameObject EndLine;
    public List<EnemyBase> Enemies;
    public List<ShapeChanger> ShapeChangers;
    public List<GameObject> Obstacles;


    public void Init()
    {
        EndLine.SetActive(true);
        foreach (var enemy in Enemies)
        {
            enemy.gameObject.SetActive(true);
            enemy.Init();
        }

        foreach (var shapeChanger in ShapeChangers)
        {
            shapeChanger.gameObject.SetActive(true);
        }

        foreach (var obstacle in Obstacles)
        {
            obstacle.SetActive(true);
        }
    }

    public void Disable()
    {
        foreach (var enemy in Enemies)
        {
            enemy.Disable();
        }

        gameObject.SetActive(false);
    }
}

public enum LevelType
{
    Start,
    Easy,
    Medium,
    Hard
}
