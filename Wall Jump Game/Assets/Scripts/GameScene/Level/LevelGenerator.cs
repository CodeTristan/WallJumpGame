using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    public delegate void OnLevelCompletedDelegate();
    public event OnLevelCompletedDelegate OnLevelCompleted;

    public float BomberSpawnChanceIn1000;
    public float DoublePointSpawnChanceIn1000;
    public float SlingShotSpawnChanceIn1000;

    [SerializeField] private List<Level> levelPool;

    private List<Level> levelInUse;
    private const int MAX_LEVEL_NUMBER = 8;
    private int completedLevelCount;
    private float levelSpawnYPosition;

    public void Init()
    {
        instance = this;
        levelSpawnYPosition = 0;
        levelInUse = new List<Level>();

        OnLevelCompleted += CompleteLevel;
        OnLevelCompleted += GenerateLevel;

        GenerateLevel();
    }

    private void OnDestroy()
    {
        OnLevelCompleted -= GenerateLevel;
    }

    public void OnLevelCompletedEvent()
    {
        OnLevelCompleted?.Invoke();
        Debug.Log("Level Completed");
    }

    private void CompleteLevel()
    {
        completedLevelCount++;
    }

    public void GenerateLevel()
    {
        DisableLevels();

        int levelsToGenerateCount = MAX_LEVEL_NUMBER - levelInUse.Count;

        for (int i = 0; i < levelsToGenerateCount; i++)
        {
            Level level = levelPool[Random.Range(0, levelPool.Count)];
            level.transform.position = new Vector2(level.SpawnOffset.x, levelSpawnYPosition + level.SpawnOffset.y);
            level.gameObject.SetActive(true);
            level.Init();
            levelInUse.Add(level);
            levelPool.Remove(level);
            levelSpawnYPosition += level.SpawnOffset.y;
        }
    }

    private void DisableLevels()
    {
        if (completedLevelCount < MAX_LEVEL_NUMBER / 2)
            return;

        int levelsToDisableCount = completedLevelCount - MAX_LEVEL_NUMBER / 2;
        completedLevelCount -= levelsToDisableCount;
        Debug.Log("Disabling Levels : " + levelsToDisableCount);

        for (int i = 0; i < levelsToDisableCount; i++)
        {
            Level level = levelInUse[0];
            level.Disable();
            levelInUse.RemoveAt(0);
            levelPool.Add(level);
        }
    }
}
