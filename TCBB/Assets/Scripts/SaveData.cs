using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class SaveData
{
	public static string path = Application.persistentDataPath+"/data.json";

	public static void saveToJson(Logic logic)
	{
		UserData data = new UserData(logic.activeProjects, logic.finishedProjects);
		string json = JsonConvert.SerializeObject(data);

		try
		{
			// Create the file, or overwrite if the file exists.
			using (FileStream fs = File.Create(path))
			{
				byte[] info = new UTF8Encoding(true).GetBytes(json);
				fs.Write(info, 0, info.Length);
			}
			Debug.Log("Data sucessfulty stored in: "+path);
		}

		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	public static UserData loadFromJson()
	{
		try
		{
			using (StreamReader sr = File.OpenText(path))
			{
				string json = sr.ReadToEnd();
				return JsonConvert.DeserializeObject<UserData>(json);
			}
		}
		catch
        {
			return null;
        }
	}
}
