using System.Collections.Generic;
using MoonSharp.Interpreter;

[MoonSharpUserData]
[System.Serializable]
public class UnitData
{
	public string name,description;
	public int maxHealth,movementRange,damage,spriteIndex,vision,expScale;

	public List<Attack> attacks;

	public string lua;
}