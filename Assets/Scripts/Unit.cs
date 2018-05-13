using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;
using Sigtrap.Relays;

public class Unit : MonoBehaviour
{
	public SpriteRenderer sr;
	public Tile prevTile;
	public Tile tile;
	[HideInInspector]
	public int health;
	public int maxHealth;
	public int movementRange, attackMaxRange, attackMinRange;
	public int movementUsed;

	public int spriteIndex;

	public int damage;

	public bool moving;
	public Pathfinding pathfinding;
	public Pathfinding rPathfinding;
	private Tile[] moveRangeTiles;
	private List<Tile> attackRangeTiles;
	private Tile moveTile;

	public GameObject attackReticule;
	public SpriteRenderer selector;
	public PathDisplay pathDisplay;

	public Unit attackTarget;

	public int player;

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

	void ResetUnit()
	{
		health = maxHealth;
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

	public void Select()
	{
		if(GameManager.instance.currentPlayer == player)
		{
			if(GameManager.instance.selectedUnit != null)
			{
				GameManager.instance.selectedUnit.Unselect();
			}
			GameManager.instance.selectedUnit = this;
			GameManager.instance.unitDisplay.LoadUnit(this);
			selector.color = GameManager.instance.players[player].unitColor;
			if (movementRange - movementUsed > 1)
			{
				moveTile = tile;
				GetMovementTiles();
				moving = true;
				GameManager.instance.state = GameManager.GameState.MOVE;
				GetAttackTiles(moveTile);
				
			}
		}
	}
	public void Undo()
	{
		SetTile(prevTile);
		movementUsed = 0;
		attackTarget = null;
		pathDisplay.hidden = true;
	}

	public void Unselect()
	{
		GameManager.instance.selectedUnit = null;
		
		selector.color = Color.grey;
	}

	void Attack()
	{
		if(attackTarget)
		{
			attackTarget.health -= damage;
			attackTarget = null;
			attackReticule.SetActive(false);
		}
	}

	void GetMovementTiles()
	{
		if (rPathfinding == null)
		{
			rPathfinding = Pathfinding.GetRange(tile, movementRange - movementUsed);
		}
		else
		{
			rPathfinding.CreateRangedMap(tile, movementRange - movementUsed);
		}
		moveRangeTiles = new Tile[rPathfinding.path.Count];
		for (int i = 0; i < rPathfinding.path.Count; i++)
		{
			moveRangeTiles[i] = GameManager.instance.GetTile(rPathfinding.path[i]);
		}
	}

	void GetAttackTiles(Tile _tile)
	{
		if (rPathfinding == null)
		{
			rPathfinding = Pathfinding.GetRange(_tile, attackMaxRange);
		}
		else
		{
			rPathfinding.CreateRangedMap(_tile, attackMaxRange);
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
		rPathfinding.CreateRangedMap(_tile, attackMinRange);
		for (int i = 0; i < attackRangeTiles.Count; i++)
		{
			for(int j = 0; j < rPathfinding.path.Count; j++)
			{
				if(attackRangeTiles[i] == GameManager.instance.GetTile(rPathfinding.path[j]))
				{
					attackRangeTiles.RemoveAt(i);
				}
			}
		}
	}

	public void Move(Tile destination)
	{
		SetTile(destination);
		GameManager.instance.state = GameManager.GameState.IDLE;
		//Unselect();
		moving = false;
		if (attackTarget)
		{
			attackReticule.transform.position = attackTarget.transform.position;
			attackReticule.gameObject.SetActive(true);
		}
		else
		{
			attackReticule.gameObject.SetActive(false);
		}
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
		bool foundGuy = false;
		for (int i = 0; i < attackRangeTiles.Count; i++)
		{
			if(attackRangeTiles[i].occupant != null)
			{
				if(attackRangeTiles[i].occupant.player != player)
				{
					attackTarget = attackRangeTiles[i].occupant;
					foundGuy = true;
				}
			}
			attackRangeTiles[i].Highlight(attackColor);
		}
		if(attackTarget)
		{
			if(!foundGuy)
			{
				attackReticule.gameObject.SetActive(false);
				attackTarget = null;
			}
			else
			{
				attackReticule.transform.position = attackTarget.transform.position;
				attackReticule.gameObject.SetActive(true);
			}
		}
		else
		{
			attackReticule.gameObject.SetActive(false);
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
					Move(moveTile);
					movementUsed += pathfinding.path[pathfinding.path.Count - 1].index;
				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if(moving)
		{
			MoveCheck();
		}
	}
}
