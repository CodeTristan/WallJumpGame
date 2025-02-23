using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject Bomber;
    public GameObject doublePoint;
    public GameObject slingShot;

    public float bomberSpawnRate;
    public float doublePointSpawnRate;
    public float slingShotSpawnRate;

    public GameObject empty;
    public GameObject[] entryLevels;
    public GameObject[] NoobLevels;
    public GameObject[] HardLevels;

    [SerializeField]
    public GameObject[,] spawnedLevels;
    public GameObject[] powerUps;


    public int levelCount;
    public int levelDeleteNumber = 5;
    public int loop = 1;
    public int difficulty = 0;

    private int index = 0;
    private int powerUpIndex = 0;
    private PlayerMovement player;
    private GameObject spawned;
    private float random;


    public void Init()
    {
        player = FindObjectOfType<PlayerMovement>();
        spawnedLevels = new GameObject[2,levelCount];
        powerUps = new GameObject[levelCount];
        MakeGame();
        
    }

    private void Update()
    {
        //if(player.completedLevels / loop >= levelCount + levelDeleteNumber)
        //{
        //    player.completedLevels -= levelDeleteNumber; // Added this because leveldeletenum adds up and levels doesnt spawn enough.
        //    deleteRest();
        //    createLevel();
        //}
    }
    public void MakeGame()
    {
        
        index = 0;
        int randomEntry = Random.Range(0, entryLevels.Length);
        spawned = Instantiate(entryLevels[randomEntry], transform.position, Quaternion.identity);
        spawnedLevels[0,0] = spawned;

        int randomNoob = Random.Range(0, NoobLevels.Length);
        spawned = Instantiate(NoobLevels[randomNoob], spawned.transform.position + new Vector3(0, 30, 0), Quaternion.identity);
        spawnedLevels[0,1] = spawned;

        for (int i = 0; i < levelCount - 2; i++)
        {
            randomNoob = Random.Range(0, NoobLevels.Length);
            spawned = Instantiate(NoobLevels[randomNoob], spawned.transform.position + new Vector3(0, 30, 0), Quaternion.identity);
            spawned.gameObject.name = i.ToString();
            spawnedLevels[0,i+2] = spawned;

            spawnSuperPowers();
        }
        index++;
        createLevel();
    }
    public  void deleteRest()
    {
        index++;
        loop++;
        
        if (index > 1)
            index = 0;
        if(difficulty < 5)
            difficulty++;
        for (int i = 0; i < levelCount; i++)
        {
            if(spawnedLevels[index,i] != empty) // Added if because it was trying to delete the original prefab
            {
                Destroy(spawnedLevels[index,i]);
                spawnedLevels[index,i] = empty;
            }

        }
        DeletePowerUps();
    }
    public void createLevel()
    {
        int randomNoob = Random.Range(0, NoobLevels.Length);
        int randomHard = Random.Range(0, HardLevels.Length);
        int randomDif = Random.Range(difficulty, 10);

        for (int i = 0; i < levelCount; i++)
        {
            if(randomDif == 9)
            {
                spawned = Instantiate(HardLevels[randomHard], spawned.transform.position + new Vector3(0, 30, 0), Quaternion.identity);
                spawned.name = i.ToString() + " HARD";
                spawnedLevels[index, i] = spawned;

                randomHard = Random.Range(0, HardLevels.Length);
            }
            else
            { 
                spawned = Instantiate(NoobLevels[randomNoob], spawned.transform.position + new Vector3(0, 30, 0), Quaternion.identity);
                spawned.name = i.ToString();
                spawnedLevels[index,i] = spawned;

                randomNoob = Random.Range(0, NoobLevels.Length);
                
            }

            randomDif = Random.Range(0, 10);
            spawnSuperPowers();
        }
    }
    private void spawnSuperPowers()
    {
        
        random = Random.Range(0, 100);
        bool powerSpawned = false;

        if (random <= bomberSpawnRate && !powerSpawned)
        {
            powerUps[powerUpIndex] = Instantiate(Bomber, spawned.transform.position + new Vector3(0, -15, 0), Quaternion.identity);
            powerUps[powerUpIndex].name = spawned.name + " Bomber";
            powerUpIndex++;
            powerSpawned = true;
            
        }

        random = Random.Range(0, 100);
        if (random <= slingShotSpawnRate && !powerSpawned)
        {
            powerUps[powerUpIndex] = Instantiate(slingShot, spawned.transform.position + new Vector3(0, -15, 0), Quaternion.identity);
            powerUps[powerUpIndex].name = spawned.name + " SlingShot";
            powerUpIndex++;
            powerSpawned = true;
        }
            
        random = Random.Range(0, 100);
        if (random <= doublePointSpawnRate && !powerSpawned)
        {
            powerUps[powerUpIndex] = Instantiate(doublePoint, spawned.transform.position + new Vector3(0, -15, 0), Quaternion.identity);
            powerUps[powerUpIndex].name = spawned.name + " doublePoint";
            powerUpIndex++;
            powerSpawned = true;
        }
            
        
    }
    public void resetGame()
    {
        difficulty = 0;
        if (index > 1)
            index = 0;
        for (int i = 0; i < 2; i++)
        {
            for(int j = 0; j < levelCount; j++)
            {
                if(spawnedLevels[i,j] != empty) // Added if because it was trying to delete the original prefab
                {
                    Destroy(spawnedLevels[i,j]);
                    spawnedLevels[i,j] = empty;
                }
            }
        }
        DeletePowerUps();
        
        loop = 1;

    }

    private void DeletePowerUps()
    {
        for (int i = 0; i < powerUpIndex; i++) 
        {
            Debug.Log(powerUps[i].name + " is destroyed");
            Destroy(powerUps[i].gameObject);
            powerUps[i] = empty;
        }
        powerUpIndex = 0;
    }

}
