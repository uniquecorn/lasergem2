[System.Serializable]
public class Attack
{
	public string name;
	public float damageScale;
	public Stats.DamageType damageType;
	public int minRange;
	public int maxRange;

	public bool aoe;
	public int aoeMinRange;
	public int aoeMaxRange;
}
