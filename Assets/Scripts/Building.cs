using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : TileObject
{
	public override void Select()
	{
		base.Select();
		if(GameManager.selectedObject == this)
		{

		}
	}
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
