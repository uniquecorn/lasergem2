using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class Tile : CastleObject
{
	public SpriteRenderer sr;
	public int x, y;
	public Unit occupant;
	public bool blocked;

	private bool highlighted;

	public override void Tap()
	{
		GameManager.instance.cameraManager.Focus(this);
		base.Tap();
		if(GameManager.instance.state == GameManager.GameState.IDLE)
		{
			if (occupant)
			{
				occupant.Select();
			}
		}
		else if(GameManager.instance.state == GameManager.GameState.MOVE)
		{

		}
	}

	public void Highlight(Color _color)
	{
		sr.color = _color;
		highlighted = true;
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
	}
}
