using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : TileObject
{
	public SpriteRenderer sr;

	public BuildingData buildingData;

	public override void Select()
	{
		base.Select();
		if(GameManager.selectedObject == this)
		{
			ShowBuildOptions();
		}
	}

	void ShowBuildOptions()
	{

	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}