using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
	// Use this for initialization
	public static List<UnitData> LoadAll ()
	{
		if(Directory.Exists(GetPath()))
		{
			List<UnitData> gameData = new List<UnitData>();
			DirectoryInfo directoryInfo = new DirectoryInfo(GetPath());
			FileInfo[] dirInfo = directoryInfo.GetFiles();
			for(int i = 0; i < dirInfo.Length; i++)
			{
				if(dirInfo[i].Extension == ".json")
				{
					gameData.Add(Load(dirInfo[i].FullName));
				}
			}
			return gameData;
		}
		else
		{
			return new List<UnitData>();
		}
	}

	public static UnitData Load(string name)
	{
		return JsonUtility.FromJson<UnitData>(File.ReadAllText(name));
	}

	public static void Save(UnitData data)
	{
		string path = GetPath() + data.name + ".json";
		string jsonData = JsonUtility.ToJson(data,true);
		File.WriteAllText(path, jsonData);
	}

	public static void Save(List<UnitData> data)
	{
		for(int i = 0; i < data.Count; i++)
		{
			Save(data[i]);
		}
	}

	public static string GetPath()
	{
		string path = Application.dataPath + "/Units/";
#if UNITY_EDITOR
		path = Application.persistentDataPath + "/Units/";
#endif
		if(!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		return path;
	}
}
