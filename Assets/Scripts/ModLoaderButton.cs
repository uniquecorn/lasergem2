using UnityEngine;
using UnityEngine.UI;

public class ModLoaderButton : MonoBehaviour
{
	public Text modName;
	public void Load(Game _mod)
	{
		modName.text = _mod.gameName;
	}
	public void LoadMod()
	{

	}
}
