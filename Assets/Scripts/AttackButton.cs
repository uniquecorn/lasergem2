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
	public Button edit, delete;
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
	}
}
