using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveData
{
    public static void saveUserData(Logic logic)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/usr.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        UserData user = new UserData(logic);
        formatter.Serialize(stream, user);
        stream.Close();
    }

    public static UserData LoadUser()
    {
        string path = Application.persistentDataPath + "/usr.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            UserData user = formatter.Deserialize(stream) as UserData;
            stream.Close();

            return user;
        }
        else
        {
            Debug.LogError("Save file not found at: "+path);
            return null;
        }
    }
}
