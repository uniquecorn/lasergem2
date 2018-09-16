[System.Serializable]
public class GameUnit
{
	public string name, description;
	public int maxHealth, movementRange, damage, vision;
	public GameSprite[] sprites;
	public GameUnitState[] states;
	public UnitAction[] action;
}