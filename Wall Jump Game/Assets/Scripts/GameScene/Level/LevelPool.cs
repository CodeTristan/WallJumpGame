using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPool : MonoBehaviour
{
    public string Name;
    public List<Level> levels;
    public List<Level> StarterLevels;


    public void Init()
    {
        foreach (Level level in levels)
        {
            level.Init();
        }

        foreach (Level level in StarterLevels)
        {
            level.Init();
        }
    }


    public void Add(Level level)
    {
        levels.Add(level);
    }

    public void Remove(Level level)
    {
        levels.Remove(level);
    }

    public Level GetRandomLevel()
    {
        return levels[Random.Range(0, levels.Count)];
    }

    public Level GetRandomStarterLevel()
    {
        return StarterLevels[Random.Range(0, StarterLevels.Count)];
    }
}
