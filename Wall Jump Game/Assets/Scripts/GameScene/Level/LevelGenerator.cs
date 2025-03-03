using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    public delegate void OnLevelCompletedDelegate();
    public event OnLevelCompletedDelegate OnLevelCompleted;

    public bool TEST_MODE;
    public float BomberSpawnChanceIn1000;
    public float DoublePointSpawnChanceIn1000;
    public float SlingShotSpawnChanceIn1000;
    public int LevelCountToIncreaseDifficulty = 10;

    public Level currentLevel;

    [SerializeField] private List<Level> levelPool;
    [SerializeField] private List<Level> StartLevelList;

    private List<Level> EasyLevelList;
    private List<Level> MediumLevelList;
    private List<Level> HardLevelList;

    private List<Level> levelInUse;
    private const int MAX_LEVEL_NUMBER = 8;
    private int completedLevelCount;
    private float levelSpawnYPosition;
    private int TotalCompletedLevelCount;

    public void Init()
    {
        instance = this;
        levelInUse = new List<Level>();

        EasyLevelList = new List<Level>();
        MediumLevelList = new List<Level>();
        HardLevelList = new List<Level>();

        OnLevelCompleted += CompleteLevel;
        OnLevelCompleted += GenerateLevel;

        foreach (Level level in levelPool)
        {
            level.Init();
        }

        if(TEST_MODE)
        {
            if (currentLevel == null)
            {
                Debug.LogError("TEST MODE IS ON BUT CURRENT LEVEL IS NULL. PUT LEVEL INTO CURRENT LEVEL TO TEST");
                return;
            }

            TestModeLevelSpawn();
            return;
        }

        Restart();
    }

    private void OnDestroy()
    {
        OnLevelCompleted -= GenerateLevel;
    }

    public void Restart()
    {
        levelSpawnYPosition = 0;
        completedLevelCount = 0;
        TotalCompletedLevelCount = 0;

        int count = levelInUse.Count;
        for (int i = 0; i < count; i++)
        {
            Level level = levelInUse[0];
            level.Disable();
            levelInUse.RemoveAt(0);
            levelPool.Add(level);
        }
        levelInUse.Clear();


        GenerateLevel();
    }

    public void OnLevelCompletedEvent()
    {
        OnLevelCompleted?.Invoke();
        Debug.Log("Level Completed");
    }

    private void CompleteLevel()
    {
        completedLevelCount++;
        TotalCompletedLevelCount++;
        currentLevel = levelInUse[completedLevelCount];
    }

    public void GenerateLevel()
    {
        DisableLevels();

        int levelsToGenerateCount = MAX_LEVEL_NUMBER - levelInUse.Count;

        for (int i = 0; i < levelsToGenerateCount; i++)
        {
            Level level = PickLevel();
            Debug.Log("Level Type: " + level.Type);
            level.transform.position = new Vector2(level.SpawnOffset.x, levelSpawnYPosition + level.SpawnOffset.y);
            level.gameObject.SetActive(true);
            level.EnableLevel();
            levelInUse.Add(level);
            levelPool.Remove(level);
            levelSpawnYPosition += level.SpawnOffset.y;
        }

        currentLevel = levelInUse[completedLevelCount];
    }

    private Level PickLevel()
    {
        // 10/10 = 1,   25/10 = 2  difficulty 2 means 2/10 chance its a hard level
        int difficulty = TotalCompletedLevelCount / LevelCountToIncreaseDifficulty;
        difficulty %= 9; //max difficulty is 9

        Level level;

        int random = Random.Range(0, 10);
        if(random < difficulty)
        {
            //HARD LEVEl
            HardLevelList.Clear();
            foreach (var item in levelPool)
            {
                if(item.Type == LevelType.Hard)
                    HardLevelList.Add(item);
            }

            if(HardLevelList.Count > 0)
            {
                level = HardLevelList[Random.Range(0, HardLevelList.Count)];
                return level;
            }
        }
        else if(random >= 8)
        {
            //EASY LEVEL
            EasyLevelList.Clear();
            foreach (var item in levelPool)
            {
                if (item.Type == LevelType.Easy)
                    EasyLevelList.Add(item);
            }

            if(EasyLevelList.Count > 0)
            {
                level = EasyLevelList[Random.Range(0, EasyLevelList.Count)];
                return level;
            }
        }
        else
        {
            //MEDIUM LEVEL
            MediumLevelList.Clear();
            foreach (var item in levelPool)
            {
                if (item.Type == LevelType.Medium)
                    MediumLevelList.Add(item);
            }

            if (MediumLevelList.Count > 0)
            {
                level = MediumLevelList[Random.Range(0, MediumLevelList.Count)];
                return level;
            }
        }

        //Unless nothing works we should return a random level
        level = levelPool[Random.Range(0,levelPool.Count)];
        return level;
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

    private void TestModeLevelSpawn()
    {
        for (int i = 0; i < 1; i++)
        {
            Level level = currentLevel;
            Debug.Log("Level Type: " + level.Type);
            level.transform.position = new Vector2(level.SpawnOffset.x, levelSpawnYPosition + level.SpawnOffset.y);
            level.gameObject.SetActive(true);
            level.EnableLevel();
            levelInUse.Add(level);
            levelPool.Remove(level);
            levelSpawnYPosition += level.SpawnOffset.y;
        }
    }
}
