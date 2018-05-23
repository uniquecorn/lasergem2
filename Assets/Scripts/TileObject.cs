using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
	public int health;
	public Tile tile;
	public int player;
	public bool neutral;
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
