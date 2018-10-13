using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UnitLoader : MonoBehaviour
{
	public GameUnit data;
	public Image icon;
	public Text dataName;

	// Use this for initialization
	public void Load ()
	{
		GameManager.instance.unitDisplay.unit.LoadData(data);
		GameManager.instance.unitDisplay.LoadUnit(GameManager.instance.unitDisplay.unit, GameManager.instance.unitDisplay.actionsVisible);
	}

	public void LoadData(GameUnit _data)
	{
		data = _data;
		icon.sprite = GameManager.instance.unitSprites[0];
		dataName.text = data.name;
	}

	public void Save()
	{
		data = GameManager.instance.unitDisplay.unit.data;
		string jsonData = JsonUtility.ToJson(data, true);
		string path = Application.dataPath + "/Units/" + data.name;
#if UNITY_EDITOR
		path = Application.persistentDataPath + "/Units/" + data.name;
#endif
		File.WriteAllText(path, jsonData);
		//GameManager.instance.LoadSavedUnits();
	}
}
