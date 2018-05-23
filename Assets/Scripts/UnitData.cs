using System.Collections.Generic;

[System.Serializable]
public class UnitData
{
	public string name,description;
	public int maxHealth,movementRange,damage,spriteIndex,cost,vision,expScale;

	public List<Attack> attacks;

	public string lua;
}