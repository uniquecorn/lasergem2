[System.Serializable]
public class GameUnit
{
	public string name, description;
	public int maxHealth, movementRange, damage, vision;
	public GameUnitState[] states;
	public UnitAction[] actions;
	public string luaPath;
}