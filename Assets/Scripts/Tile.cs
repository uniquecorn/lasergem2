using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class Tile : CastleObject
{
	public SpriteRenderer sr;
	public SpriteRenderer secondSr;
	public int x, y;
	public TileObject occupant;
	public bool blocked;

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

	public void Hurt(Stats.DamageType damageType, int value)
	{
		if(occupant)
		{
			occupant.health -= value;
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
			sr.color = Color.Lerp(sr.color, Color.white, Time.deltaTime * 10);
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
