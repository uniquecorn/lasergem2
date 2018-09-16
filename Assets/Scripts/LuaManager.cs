using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public static class LuaManager
{
	public static DynValue CallFunction(Script luaScript, string functionName, params object[] args)
	{
		object func = luaScript.Globals[functionName];

		return luaScript.Call(func, args);
	}
	public static IEnumerator CallLuaCoroutine(Script luaScript, string functionName, params object[] args)
	{
			object func = luaScript.Globals[functionName];
			if (func == null)
			{
				Debug.Log(functionName + " not used");
			}
			else
			{
				DynValue coroutine = luaScript.CreateCoroutine(func);
				coroutine.Coroutine.Resume();
				while (coroutine.Coroutine.State == CoroutineState.Suspended)
				{
					Debug.Log("waiting");
					yield return new WaitForSeconds(0.1f);
					coroutine.Coroutine.Resume();
				}
			}
	}
}
