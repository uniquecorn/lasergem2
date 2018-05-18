using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackEditor : MonoBehaviour
{
	public InputField nameInput, damageScale, minRange, maxRange, aoeMinRange, aoeMaxRange;
	public Toggle aoeToggle;
	public RectTransform editorTransform;
	public Attack attack;
	public Dropdown dmgTypeDrop;
	public GameObject aoeRange;

	public int position;

	public UnitDisplay unitDisplay;

	public bool shown;
	public bool aoeRangeShown;

	void Start()
	{
		dmgTypeDrop.ClearOptions();
		for (int i = 0; i < System.Enum.GetValues(typeof(Stats.DamageType)).Length; i++)
		{
			dmgTypeDrop.options.Add(new Dropdown.OptionData(((Stats.DamageType)i).ToString(), StyleSheet.instance.GetStyle((Stats.DamageType)i).icon));
		}
		dmgTypeDrop.RefreshShownValue();
	}
	public void Init()
	{
		attack = new Attack()
		{
			damageScale = 1.0f,
			damageType = Stats.DamageType.MELEE,
			minRange = 1,
			maxRange = 1,
			aoe = false,
			aoeMinRange = 1,
			aoeMaxRange = 1
		};
		nameInput.text = "";
		damageScale.text = "1.0";
		dmgTypeDrop.value = 0;
		minRange.text = "1";
		maxRange.text = "1";
		aoeToggle.isOn = false;
		aoeRange.gameObject.SetActive(false);
		aoeMinRange.text = "1";
		aoeMaxRange.text = "1";

	}
	public void SetName(string value)
	{
		attack.name = value;
		Save();
	}
	public void SetDamage(string value)
	{
		attack.damageScale = float.Parse(value);
		Save();
	}
	public void SetDamageType(int damageType)
	{
		attack.damageType = (Stats.DamageType)damageType;
		Save();
	}
	public void SetMinRange(string value)
	{
		attack.minRange = int.Parse(value);
		Save();
	}
	public void SetMaxRange(string value)
	{
		attack.maxRange = int.Parse(value);
		Save();
	}
	public void SetAOE(bool value)
	{
		attack.aoe = value;
		if(value)
		{
			aoeRange.gameObject.SetActive(true);
		}
		else
		{
			aoeRange.gameObject.SetActive(false);
		}
		Save();
	}
	public void SetAOEMinRange(string value)
	{
		attack.aoeMinRange = int.Parse(value);
		Save();
	}
	public void SetAOEMaxRange(string value)
	{
		attack.aoeMaxRange = int.Parse(value);
		Save();
	}
	// Use this for initialization
	public void LoadAttack (Attack _attack)
	{
		aoeRange.gameObject.SetActive(false);
		attack = _attack;
		nameInput.text = _attack.name;
		damageScale.text = _attack.damageScale.ToString();
		dmgTypeDrop.value = (int)_attack.damageType;
		minRange.text = _attack.minRange.ToString();
		maxRange.text = _attack.maxRange.ToString();
		aoeToggle.isOn = _attack.aoe;
		if(_attack.aoe)
		{
			aoeRange.gameObject.SetActive(true);
		}
		aoeMinRange.text = _attack.aoeMinRange.ToString();
		aoeMaxRange.text = _attack.aoeMaxRange.ToString();
		shown = true;
	}

	public void Save()
	{
		unitDisplay.unit.data.attacks[position] = attack;
		unitDisplay.LoadUnit(unitDisplay.unit,true);
	}

	public void Close()
	{
		shown = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(shown)
		{
			Blur.active = true;
			editorTransform.anchoredPosition = Vector2.Lerp(editorTransform.anchoredPosition, new Vector2(0, 15), Time.deltaTime * 8);
		}
		else
		{
			Blur.active = false;
			editorTransform.anchoredPosition = Vector2.Lerp(editorTransform.anchoredPosition, new Vector2(0, 500), Time.deltaTime * 8);
		}
		if(attack.aoe)
		{
			editorTransform.sizeDelta = Vector2.Lerp(editorTransform.sizeDelta, new Vector2(200, 190), Time.deltaTime * 5);
		}
		else
		{
			editorTransform.sizeDelta = Vector2.Lerp(editorTransform.sizeDelta, new Vector2(200, 160), Time.deltaTime * 5);
		}
	}
}
