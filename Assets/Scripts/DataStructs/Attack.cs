using UnityEngine;

[System.Serializable]
public class Attack : UnitAction
{
	public float damageScale;
	public Stats.DamageType damageType;

	public override void Use(Tile _tile, Unit _user)
	{
		base.Use(_tile, _user);
		_tile.Hurt(_user, damageType, Mathf.FloorToInt(_user.damage * damageScale));
	}
}
