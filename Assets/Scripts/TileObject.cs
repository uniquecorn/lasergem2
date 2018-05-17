using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
	public int health;
	public Tile tile;
	public int player;
	public virtual void Select()
	{
		if (GameManager.instance.selectedObject != null)
		{
			if(GameManager.instance.selectedObject == this)
			{
				GameManager.instance.selectedObject.Unselect();
			}
			else
			{
				GameManager.instance.selectedObject.Unselect();
				GameManager.instance.selectedObject = this;
			}
		}
		else
		{
			GameManager.instance.selectedObject = this;
		}
	}

	public virtual void Unselect()
	{

	}

	public virtual void Undo()
	{

	}
}
