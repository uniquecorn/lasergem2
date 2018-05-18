using System.Collections.Generic;

[System.Serializable]
public class UnitData
{
	public int maxHealth;
	public int movementRange;

	public int spriteIndex;

	public int damage;

	public List<Attack> attacks;
}
