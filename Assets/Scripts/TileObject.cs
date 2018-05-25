using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class TileObject : MonoBehaviour
{
	public int health;
	public Tile tile;
	public int player;
	public bool neutral;

	public enum Facing
	{
		UP,
		DOWN,
		LEFT,
		RIGHT
	}
	public Facing facing;

	public virtual void Select()
	{
		if (GameManager.selectedObject != null)
		{
			if(GameManager.selectedObject == this)
			{
				GameManager.selectedObject.Unselect();
			}
			else
			{
				GameManager.selectedObject.Unselect();
				GameManager.selectedObject = this;
			}
		}
		else
		{
			GameManager.selectedObject = this;
		}
	}

	public virtual void Unselect()
	{

	}

	public virtual void Undo()
	{

	}
}
