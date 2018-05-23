using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;
using Sigtrap.Relays;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Unit : TileObject
{
	public SpriteRenderer sr;
	public Tile prevTile;

	public enum UnitPhase
	{
		IDLE,
		MOVE,
		FACING,
		ATTACK
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
	public int damage;

	public int movementUsed;
	public bool moving;

	private int minAttackRange;
	private int maxAttackRange;
	private int currentAttack;

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

	public Tile attackTarget;

	public Color moveColor;
	public Color attackColor;

	Script luaScript;
	public bool luaLoaded;
	// Use this for initialization
	void Start ()
	{
		ResetUnit();
		GameManager.instance.endTurn.AddListener(OnTurn);
	}

	public void LoadData(UnitData _data)
	{
		data = _data;
		if (data.lua != "" && !string.IsNullOrEmpty(data.lua))
		{
			LoadLua(data.lua);
		}
		else
		{
			luaScript = null;
			luaLoaded = false;
		}
		ResetUnit();
	}

	public void LoadLua(string luaPath)
	{
		UserData.RegisterAssembly();
		print(SaveManager.GetPath() + "/" + luaPath);
		string luaCode = System.IO.File.ReadAllText(SaveManager.GetPath() + luaPath);
		luaScript = new Script();
		luaScript.DoString(luaCode);
		luaLoaded = true;
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
		unitPhase = UnitPhase.IDLE;
	}

	void ResetUnit()
	{
		health = data.maxHealth;
		damage = data.damage;
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

	public void OnTurn()
	{
		if (GameManager.instance.currentPlayer == player)
		{
			OnTurnStart();
			
		}
		else
		{
			OnTurnEnd();
			
		}
		pathDisplay.hidden = true;
		movementUsed = 0;
		prevTile = tile;
	}

	public void OnTurnStart()
	{
		selector.gameObject.SetActive(true);
		selector.color = Color.grey;

		if (luaLoaded)
		{
			DynValue result = CallFunction("OnTurnStart", this);

			if (result.Type == DataType.String)
			{
				Debug.Log(result.String);
			}
		}
	}

	public void OnTurnEnd()
	{
		attackReticule.gameObject.SetActive(false);
		selector.gameObject.SetActive(false);
		Attack();

		if (luaLoaded)
		{
			DynValue result = CallFunction("OnTurnEnd", this);

			if (result.Type == DataType.String)
			{
				Debug.Log(result.String);
			}
		}
	}

	public override void Select()
	{
		base.Select();
		if (GameManager.selectedObject == this)
		{
			
			selector.color = GameManager.instance.players[player].unitColor;
			if (GameManager.instance.currentPlayer == player)
			{
				GameManager.instance.unitDisplay.LoadUnit(this,true);
			}
			else
			{
				GameManager.instance.unitDisplay.LoadUnit(this);
			}
		}
	}

	public override void Unselect()
	{
		GameManager.selectedObject = null;
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
		attackReticule.gameObject.SetActive(false);
		movementUsed = 0;
		attackTarget = null;
		pathDisplay.hidden = true;
		GameManager.instance.unitDisplay.LoadUnit(this,true);
	}

	public void StartMove()
	{
		if (data.movementRange - movementUsed > 1)
		{
			attackTarget = null;
			attackReticule.gameObject.SetActive(false);
			unitPhase = UnitPhase.MOVE;
			moveTile = tile;
			GetMovementTiles();
			GameManager.instance.state = GameManager.GameState.DISABLESELECTING;
		}
	}

	public void StartAttack(int attack)
	{
		attackTarget = null;
		attackReticule.gameObject.SetActive(false);
		minAttackRange = data.attacks[attack].minRange;
		maxAttackRange = data.attacks[attack].maxRange;
		currentAttack = attack;
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
		unitPhase = UnitPhase.ATTACK;
		GameManager.instance.state = GameManager.GameState.DISABLESELECTING;
		GetAttackTiles(tile,false);
	}

	void Attack()
	{
		if(attackTarget)
		{
			attackTarget.Hurt(data.attacks[currentAttack].damageType, Mathf.FloorToInt(damage * data.attacks[currentAttack].damageScale));
			attackTarget = null;
			attackReticule.SetActive(false);
		}
	}

	public void Hurt(Unit aggressor, int value)
	{
		health -= value;
		if(health <= 0)
		{
			health = 0;
			Die(aggressor);
		}
		if (luaLoaded)
		{
			DynValue result = CallFunction("OnHurt", this, aggressor, value);

			if (result.Type == DataType.String)
			{
				Debug.Log(result.String);
			}
		}
	}

	public void Die(Unit aggressor)
	{
		if (luaLoaded)
		{
			DynValue result = CallFunction("OnDeath", this, aggressor);

			if (result.Type == DataType.String)
			{
				Debug.Log(result.String);
			}
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
		//GetAttackTiles(tile);
	}

	void GetAttackTiles(Tile _tile, bool coneCheck = false)
	{
		if (rPathfinding == null)
		{
			rPathfinding = Pathfinding.GetRange(_tile, maxAttackRange,true);
		}
		else
		{
			rPathfinding.CreateRangedMap(_tile, maxAttackRange, true);
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
		rPathfinding.CreateRangedMap(_tile, minAttackRange, true);
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
		
		GameManager.instance.unitDisplay.LoadUnit(this,true);
		if(prevTile.y < destination.y)
		{
			SetFacing(Facing.UP);
		}
		else if(prevTile.y > destination.y)
		{
			SetFacing(Facing.DOWN);
		}
		else if (prevTile.x > destination.x)
		{
			SetFacing(Facing.RIGHT);
		}
		else if (prevTile.x < destination.x)
		{
			SetFacing(Facing.LEFT);
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
				//GetAttackTiles(moveTile);
			}
		}
		//for (int i = 0; i < attackRangeTiles.Count; i++)
		//{
		//	attackRangeTiles[i].HighlightSecondary(attackColor);
		//}
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
						if (GameManager.instance.ValidateTile(tile.x - i, tile.y + j))
						{
							coneTiles.Add(GameManager.instance.GetTile(tile.x - i, tile.y + j));
						}
					}
				}
				break;
			case Facing.LEFT:
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
		}
	}

	bool TileInCone(Tile _tile, Facing _face)
	{
		GetFaceCone(_face, maxAttackRange);
		for(int i = 0; i < coneTiles.Count; i++)
		{
			if(_tile == coneTiles[i])
			{
				return true;
			}
		}
		return false;
	}

	void AttackCheck()
	{
		for (int i = 0; i < attackRangeTiles.Count; i++)
		{
			attackRangeTiles[i].Highlight(attackColor);
		}
		if (CastleManager.selectedObject && CastleManager.selectedObject is Tile)
		{
			bool inRange = false;
			for (int i = 0; i < attackRangeTiles.Count; i++)
			{
				if(CastleManager.selectedObject == attackRangeTiles[i])
				{
					attackTarget = attackRangeTiles[i];
					inRange = true;
					break;
				}
			}
			if(!inRange)
			{
				Unselect();
			}
			else
			{
				attackReticule.transform.position = attackTarget.transform.position - Vector3.forward;
				attackReticule.gameObject.SetActive(true);

				if(TileInCone(attackTarget,Facing.UP))
				{
					SetFacing(Facing.UP);
				}
				else if(TileInCone(attackTarget, Facing.DOWN))
				{
					SetFacing(Facing.DOWN);
				}
				else if (TileInCone(attackTarget, Facing.LEFT))
				{
					SetFacing(Facing.LEFT);
				}
				else if (TileInCone(attackTarget, Facing.RIGHT))
				{
					SetFacing(Facing.RIGHT);
				}

				unitPhase = UnitPhase.IDLE;
				GameManager.instance.state = GameManager.GameState.IDLE;
			}
		}
	}
	public DynValue CallFunction(string functionName, params object[] args)
	{
		object func = luaScript.Globals[functionName];

		return luaScript.Call(func, args);
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

				break;
			case UnitPhase.ATTACK:
				AttackCheck();
				break;
		}
	}
}