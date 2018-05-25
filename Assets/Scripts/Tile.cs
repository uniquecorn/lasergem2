using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Tile : CastleObject
{
	public SpriteRenderer sr;
	public SpriteRenderer secondSr;
	public int x, y;
	public TileObject occupant;
	public bool blocked;

	public Color tileColor = Color.white;

	public int visibility;
	public bool visibilitySet;

	private bool highlighted;
	private bool highlightedSecond;

	public override void Tap()
	{
		base.Tap();
		if(GameManager.instance.state == GameManager.GameState.IDLE)
		{
			if (occupant)
			{
				GameManager.instance.cameraManager.Focus(this);
				occupant.Select();
			}
			else
			{
				if (GameManager.selectedObject != null)
				{
					GameManager.selectedObject.Unselect();
				}
			}
		}
	}

	public void Highlight(Color _color)
	{
		sr.color = _color;
		highlighted = true;
	}
	public void HighlightSecondary(Color _color)
	{
		secondSr.color = _color;
		highlightedSecond = true;
	}

	public void SetVisibility(int _vision, bool force = false)
	{
		if(!force)
		{
			visibilitySet = true;
			if (_vision < 2)
			{
				if (_vision < visibility)
				{
					if(visibility == 2)
					{
						if (_vision == 1)
						{
							visibility = 3;
						}
						else
						{
							visibility = 2;
						}
					}
					else
					{
						return;
					}
				}
				else
				{
					visibility = _vision;
				}
			}
			else
			{
				visibility = 2;
			}
		}
		else
		{
			visibility = _vision;
			visibilitySet = false;
		}
		
		switch(visibility)
		{
			case 0:
				tileColor = Color.black;
				break;
			case 1:
				tileColor = Color.gray/2;
				break;
			case 2:
				tileColor = Color.white;
				break;
			case 3:
				//SEEN BEFORE BUT OUT OF RANGE NOW
				tileColor = Color.gray;
				break;
		}
	}

	public void GrowVisibility(int _visibility = 2)
	{
		Tile tempTile = this;
		SetVisibility(_visibility);
		for (int i = 0; i < 8; i++)
		{
			switch (i)
			{
				case 0:
					tempTile = GameManager.instance.GetTile(x, y - 1);
					break;
				case 1:
					tempTile = GameManager.instance.GetTile(x, y + 1);
					break;
				case 2:
					tempTile = GameManager.instance.GetTile(x - 1, y);
					break;
				case 3:
					tempTile = GameManager.instance.GetTile(x + 1, y);
					break;
				case 4:
					tempTile = GameManager.instance.GetTile(x - 1, y - 1);
					break;
				case 5:
					tempTile = GameManager.instance.GetTile(x - 1, y + 1);
					break;
				case 6:
					tempTile = GameManager.instance.GetTile(x + 1, y - 1);
					break;
				case 7:
					tempTile = GameManager.instance.GetTile(x + 1, y + 1);
					break;
			}
			if (tempTile)
			{
				tempTile.SetVisibility(_visibility);
				if (_visibility > 1)
				{
					tempTile.GrowVisibility(_visibility - 1);
				}
			}
		}
	}

	public void Hurt(Unit aggressor, Stats.DamageType damageType, int value)
	{
		if(occupant)
		{
			if(occupant is Unit)
			{
				((Unit)occupant).Hurt(aggressor,damageType, value);
			}
			else
			{
				occupant.health -= value;
			}
		}
	}
	// Update is called once per frame
	void LateUpdate ()
	{
		if(highlighted)
		{
			highlighted = false;
		}
		else
		{
			sr.color = Color.Lerp(sr.color, tileColor, Time.deltaTime * 10);
		}
		if (highlightedSecond)
		{
			highlightedSecond = false;
		}
		else
		{
			secondSr.color = Color.Lerp(secondSr.color, Color.clear, Time.deltaTime * 10);
		}
	}
}