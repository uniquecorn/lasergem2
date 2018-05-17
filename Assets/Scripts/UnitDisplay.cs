using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDisplay : MonoBehaviour
{
	public RectTransform displayTransform;
	public RectTransform actions;
	public Unit unit;
	public Image icon;
	public Image border;
	public InputField health,damage,movement,minrange,maxrange;
	public Button moveButton,attackButton;

	public Sprite[] unitSprites;

	public bool visible;
	public bool actionsVisible;
	// Use this for initialization
	void Start ()
	{
		
	}

	public void LoadUnit(Unit _unit)
	{
		unit = _unit;
		health.text = unit.health.ToString();
		damage.text = unit.data.damage.ToString();
		movement.text = unit.data.movementRange.ToString();
		maxrange.text = unit.data.attackMaxRange.ToString();
		minrange.text = unit.data.attackMinRange.ToString();
		icon.sprite = unit.sr.sprite;
		border.color = GameManager.instance.players[unit.player].unitColor;
		visible = true;
		if((unit.data.movementRange - unit.movementUsed) > 1)
		{
			moveButton.interactable = true;	
		}
		else
		{
			moveButton.interactable = false;
		}
	}

	public void Hide()
	{
		visible = false;
		actionsVisible = false;
	}

	public void SwapSprite()
	{
		int index = unit.data.spriteIndex + 1;
		if(index >= GameManager.instance.unitSprites.Length)
		{
			index = 0;
		}
		unit.data.spriteIndex = index;
		unit.sr.sprite = icon.sprite = GameManager.instance.unitSprites[index];
	}

	public void SetHealth(string value)
	{
		unit.health = int.Parse(value);
		unit.data.maxHealth = unit.health;
	}
	public void SetDamage(string value)
	{
		unit.data.damage = int.Parse(value);
	}
	public void SetMovement(string value)
	{
		unit.data.movementRange = int.Parse(value);
		unit.movementUsed = 0;
	}
	public void SetRange(string value)
	{
		unit.data.attackMaxRange = int.Parse(value);
	}
	public void SetMinRange(string value)
	{
		unit.data.attackMinRange = int.Parse(value);
	}

	public void Move()
	{
		unit.StartMove();
	}

	// Update is called once per frame
	void Update ()
	{
		if(visible)
		{
			displayTransform.anchoredPosition = Vector2.Lerp(displayTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 8);
		}
		else
		{
			displayTransform.anchoredPosition = Vector2.Lerp(displayTransform.anchoredPosition, Vector2.down * 150, Time.deltaTime * 8);
		}
		if(actionsVisible)
		{
			actions.anchoredPosition = Vector2.Lerp(actions.anchoredPosition, Vector2.up * 5, Time.deltaTime * 8);
		}
		else
		{
			actions.anchoredPosition = Vector2.Lerp(actions.anchoredPosition, Vector2.down * 90, Time.deltaTime * 8);
		}
	}
}
