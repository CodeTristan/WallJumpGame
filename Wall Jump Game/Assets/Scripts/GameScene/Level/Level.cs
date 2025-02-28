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

    private Dictionary<int,Vector2> StartPositions;

    public void Init()
    {
        StartPositions = new Dictionary<int, Vector2>();

        foreach (var enemy in Enemies)
        {
            StartPositions.Add(enemy.transform.GetInstanceID(), enemy.transform.localPosition);
        }

        foreach (var shapeChanger in ShapeChangers)
        {
            StartPositions.Add(shapeChanger.transform.GetInstanceID(), shapeChanger.transform.localPosition);
        }

        foreach (var obstacle in Obstacles)
        {
            StartPositions.Add(obstacle.transform.GetInstanceID(), obstacle.transform.localPosition);
        }
    }

    public void EnableLevel()
    {
        EndLine.SetActive(true);
        foreach (var enemy in Enemies)
        {
            enemy.gameObject.SetActive(true);
            enemy.transform.localPosition = StartPositions.GetValueOrDefault(enemy.transform.GetInstanceID(), Vector2.zero);
            enemy.Init();
        }

        foreach (var shapeChanger in ShapeChangers)
        {
            shapeChanger.gameObject.SetActive(true);
            shapeChanger.transform.localPosition = StartPositions.GetValueOrDefault(shapeChanger.transform.GetInstanceID(), Vector2.zero);

        }

        foreach (var obstacle in Obstacles)
        {
            obstacle.SetActive(true);
            obstacle.transform.localPosition = StartPositions.GetValueOrDefault(obstacle.transform.GetInstanceID(), Vector2.zero);

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
