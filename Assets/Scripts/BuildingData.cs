[System.Serializable]
public class BuildingData
{
	public string name, description;
	public int maxHealth,spriteIndex,vision,gpt;

	public BuildingAction[] buildingActions;

	public string lua;
}