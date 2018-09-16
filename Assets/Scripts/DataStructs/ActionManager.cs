using UnityEngine;
using MoonSharp.Interpreter;

public class ActionManager : MonoBehaviour
{
	public string luaPath;
	Script luaScript;

	public static ActionManager instance;

	private void Awake()
	{
		instance = this;
	}

	public void LoadLua()
	{
		string luaCode = System.IO.File.ReadAllText(SaveManager.GetPath() + luaPath);
		luaScript = new Script();
		luaScript.Globals["GameManager"] = typeof(GameManager);
		luaScript.DoString(luaCode);
	}

	public void UseAction(UnitAction _action, Tile _tile, Unit _owner)
	{
		LuaManager.CallFunction(luaScript, _action.actionName, _tile, _owner);
	}
}
