using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;
using Sigtrap.Relays;

public class Unit : TileObject
{
	public SpriteRenderer sr;
	public Tile prevTile;

	public enum UnitPhase
	{
		IDLE,
		MOVE,
		FACING
	}

	public UnitPhase unitPhase;

	public enum Facing
	{
		UP,
		DOWN,
		LEFT,
		RIGHT
	}
	public Facing facing;

	public UnitData data;

	public int movementUsed;
	public bool moving;

	public Pathfinding pathfinding;
	public Pathfinding rPathfinding;
	private Tile[] moveRangeTiles;
	private List<Tile> attackRangeTiles;
	private Tile moveTile;
	private Tile faceTile;
	private List<Tile> faceTiles;
	private List<Tile> coneTiles;

	public GameObject attackReticule;
	public GameObject faceIndicator;
	public SpriteRenderer selector;
	public PathDisplay pathDisplay;

	public TileObject attackTarget;

	public Color moveColor;
	public Color attackColor;

	// Use this for initialization
	void Start ()
	{
		ResetUnit();
		GameManager.instance.endTurn.AddListener(OnTurnStart);
	}

	public void SetTile(Tile _tile)
	{
		if(tile)
		{
			tile.occupant = null;
		}
		tile = _tile;
		tile.occupant = this;
		transform.position = new Vector3(tile.x, tile.y, -0.1f);
	}

	public void SetFacing(Facing _face)
	{
		facing = _face;
		switch(facing)
		{
			case Facing.UP:
				faceIndicator.transform.rotation = Quaternion.identity;
				break;
			case Facing.DOWN:
				faceIndicator.transform.rotation = Quaternion.Euler(0,0,180);
				break;
			case Facing.LEFT:
				faceIndicator.transform.rotation = Quaternion.Euler(0, 0, 270);
				break;
			case Facing.RIGHT:
				faceIndicator.transform.rotation = Quaternion.Euler(0, 0, 90);
				break;
		}
		Unselect();
	}

	void ResetUnit()
	{
		health = data.maxHealth;
		prevTile = tile;
		sr.color = GameManager.instance.players[player].unitColor;
		if (GameManager.instance.currentPlayer == player)
		{
			selector.gameObject.SetActive(true);
			selector.color = Color.grey;
		}
		else
		{
			selector.gameObject.SetActive(false);
		}
	}

	public void OnTurnStart()
	{
		if (GameManager.instance.currentPlayer == player)
		{
			selector.gameObject.SetActive(true);
			selector.color = Color.grey;
		}
		else
		{
			selector.gameObject.SetActive(false);
		}
		pathDisplay.hidden = true;
		movementUsed = 0;
		prevTile = tile;
		Attack();
	}

	public override void Select()
	{
		base.Select();
		if (GameManager.instance.selectedObject == this)
		{
			GameManager.instance.unitDisplay.LoadUnit(this);
			selector.color = GameManager.instance.players[player].unitColor;
			if (GameManager.instance.currentPlayer == player)
			{
				GameManager.instance.unitDisplay.actionsVisible = true;
			}
		}
	}

	public override void Unselect()
	{
		GameManager.instance.selectedObject = null;
		selector.color = Color.grey;
		GameManager.instance.unitDisplay.Hide();
		unitPhase = UnitPhase.IDLE;
		GameManager.instance.state = GameManager.GameState.IDLE;
		if(movementUsed == 0)
		{
			pathDisplay.hidden = true;
		}
	}

	public override void Undo()
	{
		SetTile(prevTile);
		movementUsed = 0;
		attackTarget = null;
		pathDisplay.hidden = true;
		GameManager.instance.unitDisplay.LoadUnit(this);
	}

	public void StartMove()
	{
		if (data.movementRange - movementUsed > 1)
		{
			unitPhase = UnitPhase.MOVE;
			moveTile = tile;
			GetMovementTiles();
			GameManager.instance.state = GameManager.GameState.DISABLESELECTING;
		}
	}

	public void StartFacing()
	{
		unitPhase = UnitPhase.FACING;
		GameManager.instance.state = GameManager.GameState.DISABLESELECTING;
		GetFaceCone(facing, data.attackMaxRange);
		faceTiles = new List<Tile>();
		int _x = tile.x;
		int _y = tile.y;
		if (_y + 1 <= GameManager.MAP_HEIGHT)
		{
			faceTiles.Add(GameManager.instance.GetTile(_x, _y + 1));
		}
		if (_y - 1 >= 0)
		{
			faceTiles.Add(GameManager.instance.GetTile(_x, _y - 1));
		}
		if (_x - 1 >= 0)
		{
			faceTiles.Add(GameManager.instance.GetTile(_x - 1, _y));
		}
		if (_x + 1 <= GameManager.MAP_WIDTH)
		{
			faceTiles.Add(GameManager.instance.GetTile(_x + 1, _y));
		}
	}

	void Attack()
	{
		if(attackTarget)
		{
			attackTarget.health -= data.damage;
			attackTarget = null;
			attackReticule.SetActive(false);
		}
	}

	void GetMovementTiles()
	{
		if (rPathfinding == null)
		{
			rPathfinding = Pathfinding.GetRange(tile, data.movementRange - movementUsed);
		}
		else
		{
			rPathfinding.CreateRangedMap(tile, data.movementRange - movementUsed);
		}
		moveRangeTiles = new Tile[rPathfinding.path.Count];
		for (int i = 0; i < rPathfinding.path.Count; i++)
		{
			moveRangeTiles[i] = GameManager.instance.GetTile(rPathfinding.path[i]);
		}
		GetAttackTiles(tile);
	}

	void GetAttackTiles(Tile _tile, bool coneCheck = false)
	{
		if (rPathfinding == null)
		{
			rPathfinding = Pathfinding.GetRange(_tile, data.attackMaxRange,true);
		}
		else
		{
			rPathfinding.CreateRangedMap(_tile, data.attackMaxRange,true);
		}
		if(attackRangeTiles == null)
		{
			attackRangeTiles = new List<Tile>();
		}
		else
		{
			attackRangeTiles.Clear();
		}
		for (int i = 0; i < rPathfinding.path.Count; i++)
		{
			attackRangeTiles.Add(GameManager.instance.GetTile(rPathfinding.path[i]));
		}
		rPathfinding.CreateRangedMap(_tile, data.attackMinRange, true);
		print(attackRangeTiles.Count);
		for (int i = attackRangeTiles.Count - 1; i >= 0; i--)
		{
			bool removed = false;
			for(int j = 0; j < rPathfinding.path.Count; j++)
			{
				if(attackRangeTiles[i] == GameManager.instance.GetTile(rPathfinding.path[j]))
				{
					attackRangeTiles.RemoveAt(i);
					removed = true;
					break;
				}
			}

			if(!removed && coneCheck)
			{
				bool inCone = false;
				for (int k = 0; k < coneTiles.Count; k++)
				{
					if (attackRangeTiles[i] == coneTiles[k])
					{
						inCone = true;
						break;
					}
				}
				if(!inCone)
				{
					attackRangeTiles.RemoveAt(i);
				}
			}
		}
	}

	public void Move(Tile destination)
	{
		SetTile(destination);
		//unitPhase = UnitPhase.IDLE;
		//GameManager.instance.state = GameManager.GameState.IDLE;
		
		GameManager.instance.unitDisplay.LoadUnit(this);
		StartFacing();
	}

	bool CheckRange(Tile tileToCheck)
	{
		if(tileToCheck == tile || tileToCheck.occupant != null)
		{
			return false;
		}
		for (int i = 0; i < moveRangeTiles.Length; i++)
		{
			if (tileToCheck == moveRangeTiles[i])
			{
				return true;
			}
		}
		return false;
	}

	void MoveCheck()
	{
		for(int i = 0; i < moveRangeTiles.Length; i++)
		{
			moveRangeTiles[i].Highlight(moveColor);
		}
		if(CastleManager.hoveredObject is Tile)
		{
			if(CheckRange((Tile)CastleManager.hoveredObject))
			{
				moveTile = (Tile)CastleManager.hoveredObject;
				GetAttackTiles(moveTile);
			}
		}
		for (int i = 0; i < attackRangeTiles.Count; i++)
		{
			attackRangeTiles[i].HighlightSecondary(attackColor);
		}
		if (moveTile != tile)
		{
			if(pathfinding == null)
			{
				pathfinding = Pathfinding.GetPath(tile, moveTile);
			}
			else
			{
				pathfinding.Create(tile, moveTile);
			}
			
			pathDisplay.Display(pathfinding);
			pathDisplay.line.startColor = pathDisplay.line.endColor = GameManager.instance.players[player].unitColor;
			if (CastleManager.selectedObject && CastleManager.selectedObject is Tile)
			{
				if(CastleManager.selectedObject == tile)
				{
					Unselect();
				}
				else if (CheckRange((Tile)CastleManager.selectedObject))
				{
					movementUsed += pathfinding.path[pathfinding.path.Count - 1].index;
					Move(moveTile);
				}
			}
		}
	}

	void GetFaceCone(Facing _face, int range = 2)
	{
		coneTiles = new List<Tile>();
		switch(_face)
		{
			case Facing.UP:
				for(int i = 0; i < range; i++)
				{
					for (int j = -i; j <= i; j++)
					{
						if(GameManager.instance.ValidateTile(tile.x + j, tile.y + i))
						{
							coneTiles.Add(GameManager.instance.GetTile(tile.x + j, tile.y + i));
						}
					}
				}
				break;
			case Facing.DOWN:
				for (int i = 0; i < range; i++)
				{
					for (int j = -i; j <= i; j++)
					{
						if (GameManager.instance.ValidateTile(tile.x + j, tile.y - i))
						{
							coneTiles.Add(GameManager.instance.GetTile(tile.x + j, tile.y - i));
						}
					}
				}
				break;
			case Facing.RIGHT:
				for (int i = 0; i < range; i++)
				{
					for (int j = -i; j <= i; j++)
					{
						if (GameManager.instance.ValidateTile(tile.x + i, tile.y + j))
						{
							coneTiles.Add(GameManager.instance.GetTile(tile.x + i, tile.y + j));
						}
					}
				}
				break;
			case Facing.LEFT:
				for (int i = 0; i < range; i++)
				{
					for (int j = -i; j <= i; j++)
					{
						if (GameManager.instance.ValidateTile(tile.x - i, tile.y + j))
						{
							coneTiles.Add(GameManager.instance.GetTile(tile.x - i, tile.y + j));
						}
					}
				}
				break;
		}
	}

	void FaceCheck()
	{
		if (CastleManager.hoveredObject is Tile && CastleManager.hoveredObject != faceTile)
		{
			for (int i = 0; i < faceTiles.Count; i++)
			{
				if (CastleManager.hoveredObject == faceTiles[i])
				{
					faceTile = (Tile)CastleManager.selectedObject;
					if (faceTiles[i].x > tile.x)
					{
						GetFaceCone(Facing.RIGHT, data.attackMaxRange);
					}
					else if (faceTiles[i].x < tile.x)
					{
						GetFaceCone(Facing.LEFT, data.attackMaxRange);
					}
					else
					{
						if (faceTiles[i].y > tile.y)
						{
							GetFaceCone(Facing.UP, data.attackMaxRange);
						}
						else if (faceTiles[i].y < tile.y)
						{
							GetFaceCone(Facing.DOWN, data.attackMaxRange);
						}
					}
					GetAttackTiles(tile,true);
					break;
				}
			}
		}
		bool foundEnemy = false;
		for (int i = 0; i < attackRangeTiles.Count; i++)
		{
			if(attackRangeTiles[i].occupant && !foundEnemy)
			{
				if(attackRangeTiles[i].occupant.player != player)
				{
					foundEnemy = true;
					attackTarget = attackRangeTiles[i].occupant;
				}
			}
			attackRangeTiles[i].Highlight(attackColor);
		}
		if(!foundEnemy)
		{
			attackTarget = null;
		}
		if (attackTarget)
		{
			attackReticule.transform.position = attackTarget.transform.position;
			attackReticule.gameObject.SetActive(true);
		}
		else
		{
			attackReticule.gameObject.SetActive(false);
		}
		for (int i = 0; i < faceTiles.Count; i++)
		{
			faceTiles[i].HighlightSecondary(moveColor);
		}
		if (CastleManager.selectedObject && CastleManager.selectedObject is Tile)
		{
			if (CastleManager.selectedObject == tile)
			{
				//Unselect();
			}
			else
			{
				for (int i = 0; i < faceTiles.Count; i++)
				{
					if(CastleManager.selectedObject == faceTiles[i])
					{
						if(faceTiles[i].x > tile.x)
						{
							SetFacing(Facing.LEFT);
						}
						else if(faceTiles[i].x < tile.x)
						{
							SetFacing(Facing.RIGHT);
						}
						else
						{
							if (faceTiles[i].y > tile.y)
							{
								SetFacing(Facing.UP);
							}
							else if (faceTiles[i].y < tile.y)
							{
								SetFacing(Facing.DOWN);
							}
						}
						break;
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		switch(unitPhase)
		{
			case UnitPhase.IDLE:

				break;
			case UnitPhase.MOVE:
				MoveCheck();
				break;
			case UnitPhase.FACING:
				FaceCheck();
				break;
		}
	}
}
