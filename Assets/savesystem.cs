using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class savesystem
    { 

    public static void SaveUnit(Unit unit)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Unit.test";
        FileStream stream = new FileStream(path, FileMode.Create);

        data data = new data(unit);

        formatter.Serialize(stream, data);
        stream.Close();
    }


    public static data loadunit()
    {
        string path = Application.persistentDataPath + "/Unit.test"; 
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data Data =  formatter.Deserialize(stream) as data;
            stream.Close();

            return Data;
        }
        else
        {
            Debug.LogError("saved unit data not found in" + path);
            return null;
        }
    }
}
