using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDisplay : MonoBehaviour
{
	public RectTransform displayTransform;

	public RectTransform unitsScroll;

	public RectTransform actions;
	public RectTransform addAttack;
	public List<AttackButton> attacks;
	public GameObject attackButtonPrefab;
	public AttackEditor attackEditor;

	public Unit unit;
	public Image icon;
	public Image border;
	public InputField nameInput,health,damage,movement;
	public Button moveButton;
	public Button saveButton;

	public Sprite[] unitSprites;

	public bool visible;
	public bool actionsVisible;
	// Use this for initialization
	void Start ()
	{
		attacks = new List<AttackButton>();
	}

	public void SetMode()
	{
		if(GameManager.instance.mode == GameManager.GameMode.EDITOR)
		{
			nameInput.interactable = health.interactable = damage.interactable = movement.interactable = false;
			addAttack.gameObject.SetActive(false);
			saveButton.gameObject.SetActive(false);
			for(int i = 0; i < attacks.Count; i++)
			{
				attacks[i].edit.gameObject.SetActive(false);
				attacks[i].delete.gameObject.SetActive(false);
			}
		}
		else
		{
			nameInput.interactable = health.interactable = damage.interactable = movement.interactable = false;
			addAttack.gameObject.SetActive(false);
			saveButton.gameObject.SetActive(false);
			for (int i = 0; i < attacks.Count; i++)
			{
				attacks[i].edit.gameObject.SetActive(false);
				attacks[i].delete.gameObject.SetActive(false);
			}
		}
	}

	public void LoadUnit(Unit _unit,bool _actionsVisible = false)
	{
		unit = _unit;
		nameInput.text = unit.data.name;
		health.text = unit.health.ToString();
		damage.text = unit.damage.ToString();
		movement.text = unit.data.movementRange.ToString();
		icon.sprite = unit.sr.sprite = GameManager.instance.unitSprites[unit.data.spriteIndex];
		border.color = GameManager.instance.players[unit.player].unitColor;
		visible = true;
		actionsVisible = _actionsVisible;
		if((unit.data.movementRange - unit.movementUsed) > 1)
		{
			moveButton.interactable = true;	
		}
		else
		{
			moveButton.interactable = false;
		}
		if(_actionsVisible)
		{
			if (unit.data.attacks == null)
			{
				unit.data.attacks = new List<Attack>();
			}
			for (int i = 0; i < unit.data.attacks.Count; i++)
			{
				InitAttack(i);
			}
			addAttack.anchoredPosition = new Vector2(0, 60 + (unit.data.attacks.Count * 30));
		}
		else
		{
			for (int i = 0; i < attacks.Count; i++)
			{
				attacks[i].gameObject.SetActive(false);
			}
		}
		SetMode();
	}

	public void InitAttack(int pos)
	{
		Attack _attack = unit.data.attacks[pos];
		if(pos < attacks.Count)
		{
			attacks[pos].gameObject.SetActive(true);
		}
		else
		{
			CreateAttackButton(pos);
		}
		attacks[pos].editorTransform.anchoredPosition = new Vector2(0, 60 + (pos * 30));
		attacks[pos].editorTransform.anchoredPosition = new Vector2(0, 60 + (pos * 30));
		attacks[pos].attackName.text = _attack.name;
		attacks[pos].icon.sprite = StyleSheet.instance.GetStyle(_attack.damageType).icon;
		attacks[pos].position = pos;
	}

	public AttackButton CreateAttackButton(int pos)
	{
		AttackButton tempButton = Instantiate(attackButtonPrefab, actions).GetComponent<AttackButton>();
		tempButton.unitDisplay = this;
		tempButton.position = pos;
		attacks.Add(tempButton);
		return tempButton;
	}

	public void AddAttack()
	{
		if(attackEditor.shown)
		{
			return;
		}
		attackEditor.position = unit.data.attacks.Count;
		attackEditor.shown = true;
		unit.data.attacks.Add(new Attack());
		attackEditor.Init();
	}

	public void Hide()
	{
		visible = false;
		actionsVisible = false;
		attackEditor.shown = false;
		for (int i = 0; i < attacks.Count; i++)
		{
			attacks[i].gameObject.SetActive(false);
		}
	}

	public void SwapSprite()
	{
		if (attackEditor.shown)
		{
			return;
		}
		int index = unit.data.spriteIndex + 1;
		if(index >= GameManager.instance.unitSprites.Length)
		{
			index = 0;
		}
		unit.data.spriteIndex = index;
		unit.sr.sprite = icon.sprite = GameManager.instance.unitSprites[index];
	}
	public void SetName(string value)
	{
		unit.data.name = value;
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

	public void Move()
	{
		if (attackEditor.shown)
		{
			return;
		}
		unit.StartMove();
	}

	public void Undo()
	{
		if (attackEditor.shown)
		{
			return;
		}
		unit.Undo();
	}

	public void Save()
	{
		SaveManager.Save(unit.data);
		GameManager.instance.LoadSavedUnits();
	}

	// Update is called once per frame
	void Update ()
	{
		if (GameManager.instance.mode == GameManager.GameMode.EDITOR)
		{
			unitsScroll.anchoredPosition = new Vector2(0, -35);
		}
		else
		{
			unitsScroll.anchoredPosition = new Vector2(80, -35);
		}
		if (visible)
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
			actions.anchoredPosition = Vector2.Lerp(actions.anchoredPosition, Vector2.down * 120, Time.deltaTime * 8);
		}
	}
}
