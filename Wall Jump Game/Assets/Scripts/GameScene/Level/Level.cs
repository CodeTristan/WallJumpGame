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
    public List<CoinObject> CoinObjects;

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

        foreach (var obstacle in CoinObjects)
        {
            StartPositions.Add(obstacle.transform.GetInstanceID(), obstacle.transform.localPosition);
        }
    }

    public void EnableLevel()
    {
        EndLine.SetActive(true);
        Debug.Log("Level Enabling: " + gameObject.name);
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

        foreach (var obstacle in CoinObjects)
        {
            obstacle.gameObject.SetActive(true);
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

    [ContextMenu("Set Properties")]
    public void SetProperties()
    {
        Enemies.Clear();
        foreach (var enemy in GetComponentsInChildren<EnemyBase>())
        {
            if(enemy.gameObject.name != "LaserBeam")
            Enemies.Add(enemy);
        }
        ShapeChangers.Clear();
        foreach (var shapeChanger in GetComponentsInChildren<ShapeChanger>())
        {
            ShapeChangers.Add(shapeChanger);
        }
        CoinObjects.Clear();
        foreach (var obstacle in GetComponentsInChildren<CoinObject>())
        {
            CoinObjects.Add(obstacle);
        }
    }
}

public enum LevelType
{
    Start,
    Easy,
    Medium,
    Hard,
    Hardcore
}
