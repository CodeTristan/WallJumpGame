using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerData playerData;

    public SaveData()
    {
        playerData = new PlayerData();
    }
}
