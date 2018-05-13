using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDisplay : MonoBehaviour
{
	public RectTransform displayTransform;
	public Unit unit;
	public Image icon;
	public Image border;
	public InputField health,damage,movement,minrange,maxrange;

	public Sprite[] unitSprites;

	public bool visible;

	// Use this for initialization
	void Start ()
	{
		
	}

	public void LoadUnit(Unit _unit)
	{
		unit = _unit;
		health.text = unit.health.ToString();
		damage.text = unit.damage.ToString();
		movement.text = unit.movementRange.ToString();
		maxrange.text = unit.attackMaxRange.ToString();
		minrange.text = unit.attackMinRange.ToString();
		icon.sprite = unit.sr.sprite;
		border.color = GameManager.instance.players[unit.player].unitColor;
		visible = true;
	}

	public void SwapSprite()
	{
		int index = unit.spriteIndex + 1;
		if(index >= GameManager.instance.unitSprites.Length)
		{
			index = 0;
		}
		unit.spriteIndex = index;
		unit.sr.sprite = icon.sprite = GameManager.instance.unitSprites[index];
	}

	public void SetHealth(string value)
	{
		unit.health = int.Parse(value);
		unit.maxHealth = unit.health;
	}
	public void SetDamage(string value)
	{
		unit.damage = int.Parse(value);
	}
	public void SetMovement(string value)
	{
		unit.movementRange = int.Parse(value);
		unit.movementUsed = 0;
	}
	public void SetRange(string value)
	{
		unit.attackMaxRange = int.Parse(value);
	}
	public void SetMinRange(string value)
	{
		unit.attackMinRange = int.Parse(value);
	}

	// Update is called once per frame
	void Update ()
	{
		if(visible)
		{
			displayTransform.anchoredPosition = Vector2.Lerp(displayTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 5);
		}
		else
		{
			displayTransform.anchoredPosition = Vector2.Lerp(displayTransform.anchoredPosition, Vector2.down * 150, Time.deltaTime * 5);
		}
	}
}
