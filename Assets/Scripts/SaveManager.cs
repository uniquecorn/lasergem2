using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
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
	public static List<Game> GetMods()
	{
		
	}
}
