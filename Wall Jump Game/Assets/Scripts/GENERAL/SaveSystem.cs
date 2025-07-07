using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;


public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    public GameData GameData;

    private string savePath;
    private const string FILE_NAME = "SaveFile.sav";

    public void Init()
    {
        instance = this;
        savePath = Application.persistentDataPath;
        GameData = new GameData();

        LoadData();
    }

    private void LoadData()
    {
        if (File.Exists(savePath + "/" + FILE_NAME) == false)
        {
            Debug.Log("Save File not found. Creating New");
            SaveData();
            return;
        }

        Debug.Log("Save File Found. Loading...");
        var binaryFormatter = new BinaryFormatter();
        var fileStream = File.Open(savePath + "/" + FILE_NAME, FileMode.Open);
        GameData = (GameData    )binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
        Debug.Log("Data Loaded");
    }

    public void SaveData()
    {
        Debug.Log("Saving Data...");
        var binaryFormatter = new BinaryFormatter();
        var fileStream = File.Create(savePath + "/" + FILE_NAME);
        binaryFormatter.Serialize(fileStream, GameData);
        fileStream.Close();
        Debug.Log("Data Saved");
    }

}
