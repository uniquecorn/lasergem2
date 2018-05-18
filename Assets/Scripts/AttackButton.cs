using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour
{
	public RectTransform editorTransform;
	public UnitDisplay unitDisplay;
	public int position;
	public Image icon;
	public Text attackName;
	// Use this for initialization
	public void Use ()
	{
		if (unitDisplay.attackEditor.shown)
		{
			return;
		}
		unitDisplay.unit.StartAttack(position);
	}
	
	public void Delete()
	{
		if (unitDisplay.attackEditor.shown)
		{
			return;
		}
		unitDisplay.unit.data.attacks.RemoveAt(position);
		gameObject.SetActive(false);
		unitDisplay.LoadUnit(unitDisplay.unit, true);
	}

	public void Edit()
	{
		if (unitDisplay.attackEditor.shown)
		{
			return;
		}
		unitDisplay.attackEditor.position = position;
		unitDisplay.attackEditor.LoadAttack(unitDisplay.unit.data.attacks[position]);
	}
}
