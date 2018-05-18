using UnityEngine;

public class StyleSheet : MonoBehaviour
{
	[System.Serializable]
	public struct Style
	{
		public Stats.DamageType damageType;
		public Sprite icon;
		public Color color;
	}
	public Style[] styles;

	public static StyleSheet instance;

	private void Awake()
	{
		instance = this;
	}
	public Style GetStyle(Stats.DamageType _damageType)
	{
		for (int i = 0; i < styles.Length; i++)
		{
			if (styles[i].damageType == _damageType)
			{
				return styles[i];
			}
		}
		print("No style found!");
		return styles[0];
	}
}
