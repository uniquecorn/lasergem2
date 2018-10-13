using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModLoader : MonoBehaviour
{
	public ModLoaderButton modButtonPrefab;
	public ScrollRect modList;
	
	public void Init ()
	{
		List<Game> mods = SaveManager.GetMods();
		if(mods != null)
		{
			for(int i = 0; i < mods.Count; i++)
			{
				CreateModButton(mods[i]);
			}
		}
	}

	public void CreateModButton(Game mod)
	{
		Instantiate(modButtonPrefab, modList.content).Load(mod);
	}

	public void LoadMod()
	{

	}
}
