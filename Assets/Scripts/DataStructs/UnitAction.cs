using MoonSharp.Interpreter;

[MoonSharpUserData]
[System.Serializable]
public class UnitAction
{
	public string actionName;
	public string spriteID;
	public int minRange;
	public int maxRange;
	public bool aoe;
	public int aoeMinRange;
	public int aoeMaxRange;

	public virtual void Use(Tile _tile, Unit _user)
	{
		ActionManager.instance.UseAction(this, _tile, _user);
		if(aoe)
		{
			//Call Use on all the aoe tiles
		}
	}
}
