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

	public static void saveToJsonNoNewton(Logic logic)
	{
		string data = "{\"activeProjects\": [";
		foreach (Project p in logic.activeProjects)
		{
			string projectJson = JsonUtility.ToJson(p);
			projectJson = projectJson.Substring(0, projectJson.Length-1);

			projectJson += ", \"incompleteTasks\": [";
			if (p.incompleteTasks.Count > 0)
			{
				foreach (Task t in p.incompleteTasks)
				{
					string taskJson = JsonUtility.ToJson(t);
					taskJson = taskJson.Substring(0, taskJson.Length-1);
					taskJson += ", \"dateToDo\": "+t.dateToDo.ToBinary()+"}, ";
					projectJson += taskJson;
				}
				projectJson = projectJson.Substring(0, projectJson.Length-2);
			}
			projectJson += "], \"completedTasks\": [";

			if (p.completedTasks.Count > 0)
			{
				
				foreach (Task t in p.completedTasks)
				{
					string taskJson = JsonUtility.ToJson(t);
					taskJson = taskJson.Substring(0, taskJson.Length-1);
					taskJson += ", \"dateToDo\": "+t.dateToDo.ToBinary()+"}, ";
					projectJson += taskJson;
				}
				projectJson = projectJson.Substring(0, projectJson.Length-2);
			}
			projectJson += "]}, ";
			data += projectJson;
		}
		data = data.Substring(0, data.Length-2);
		data += "]}";
		
		try
		{
			// Create the file, or overwrite if the file exists.
			using (FileStream fs = File.Create(path))
			{
				byte[] info = new UTF8Encoding(true).GetBytes(data);
				// Add some information to the file.
				fs.Write(info, 0, info.Length);
			}
			
			Debug.Log("Data sucessfulty stored in: "+path);
		}
		
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

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
