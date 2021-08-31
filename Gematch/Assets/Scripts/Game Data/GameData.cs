using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] isActiveArr;
    public int[] scoresArr;
    public int[] starsArr;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;

    void Awake()
    {
        //making sure there is only one of this classes as objects
        if(gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();
    }

    public void Save()
    {
        //create a binary formater to read
        BinaryFormatter formatter = new BinaryFormatter();

        //create routh from program to file
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);

        //create copy of my save
        SaveData data = new SaveData();
        data = saveData;

        //save the data in the file
        formatter.Serialize(file, data);

        //close dataStream
        file.Close();
    }

    public void Load()
    {
        //see if there is a save file
        if(File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            //create binary formater
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();

        }
    }

    private void OnDisable()
    {
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
