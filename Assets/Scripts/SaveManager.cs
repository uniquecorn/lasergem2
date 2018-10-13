using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
	public static string loadedMod;
	private static List<Game> loadedMods;
	public static string GetModsPath()
	{
		string path = Application.streamingAssetsPath + "/Mods";

		if(!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		
		return path;
	}
	// Use this for initialization
	public static string GetPath(Game _game)
	{
		string path = Path.Combine(GetModsPath(),_game.gameName);
		return path;
	}
	public static string GetCurrentPath()
	{
		string path = Path.Combine(GetModsPath(), loadedMod);
		return path;
	}
	public static List<Game> GetMods()
	{
		if(loadedMods != null)
		{
			return loadedMods;
		}
		if (Directory.Exists(GetModsPath()))
		{
			loadedMods = new List<Game>();
			DirectoryInfo directoryInfo = new DirectoryInfo(GetModsPath());
			DirectoryInfo[] dirInfo = directoryInfo.GetDirectories();
			for (int i = 0; i < dirInfo.Length; i++)
			{
				loadedMods.Add(Load(dirInfo[i].Name));
			}
			return loadedMods;
		}
		else
		{
			return null;
		}
	}
	public static Game Load(string _name)
	{
		string _path = Path.Combine(GetModsPath(), _name);
		if (Directory.Exists(_path))
		{
			string json = File.ReadAllText(Path.Combine(_path, _name + ".json"));
			Game tempData = JsonUtility.FromJson<Game>(json);
			return tempData;
		}
		else
		{
			return null;
		}
	}
}
