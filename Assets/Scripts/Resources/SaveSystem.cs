using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/save";
        FileStream stream = new FileStream(path, FileMode.Create);
        SavedData vars = new SavedData();

        formatter.Serialize(stream, vars);
        stream.Close();
    }

    public static SavedData Load()
    {
        string path = Application.persistentDataPath + "/save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SavedData vars = formatter.Deserialize(stream) as SavedData;
            stream.Close();

            return vars;
        }
        else
        {
            Debug.Log("Save File not found in " + path);
            return null;
        }
    }
   
}
