using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Castle;
using Sigtrap.Relays;

public class GameManager : MonoBehaviour
{
	public static int MAP_WIDTH, MAP_HEIGHT;
	public int numUnits;

	public Tile[,] map;

	public GameObject tilePrefab;
	public GameObject unitPrefab;

	public Transform mapTransform;

	public enum GameState
	{
		UNINIT,
		IDLE,
		MOVE,
		ATTACK
	}
	public GameState state;

	public Player[] players;
	public int currentPlayer;
	public Text turnDisplay;

	public Unit selectedUnit;

	public static GameManager instance;
	// SINGLETONS
	public CameraManager cameraManager;
	public PathDisplay pathDisplay;
	public UnitDisplay unitDisplay;

	public Text widthHandle, heightHandle, unitHandle;
	public CanvasGroup initMap;
	public CanvasGroup gameUI;

	public Relay endTurn;

	private void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start ()
	{
		endTurn = new Relay();
		CastleManager.Init();
		SetWidth(5);
		SetHeight(5);
		SetUnits(1);
		gameUI.alpha = 0;
		initMap.alpha = 1;
		//CastleManager.showLog = true;
		//CreateMap(5, 5);
		//cameraManager.Snap(2f, 2f);
	}

	public void SetWidth(System.Single _width)
	{
		MAP_WIDTH = (int)_width;
		widthHandle.text = MAP_WIDTH.ToString();
	}

	public void SetHeight(System.Single _height)
	{
		MAP_HEIGHT = (int)_height;
		heightHandle.text = MAP_HEIGHT.ToString();
	}

	public void SetUnits(System.Single _num)
	{
		numUnits = (int)_num;
		unitHandle.text = numUnits.ToString();
	}

	public void Init()
	{
		initMap.interactable = false;
		state = GameState.IDLE;
		CreateMap(MAP_WIDTH, MAP_HEIGHT);
		cameraManager.Snap((float)(MAP_WIDTH - 1) / 2, (float)(MAP_HEIGHT - 1) / 2);
	}

	public void CreateMap(int width, int height)
	{
		MAP_WIDTH = width;
		MAP_HEIGHT = height;
		map = new Tile[width, height];
		for(int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				CreateTile(x, y);
			}
		}
		for(int i = 0; i < numUnits; i++)
		{
			CreateUnit(Random.Range(0, MAP_WIDTH), Random.Range(0, MAP_HEIGHT), 0);
			CreateUnit(Random.Range(0, MAP_WIDTH), Random.Range(0, MAP_HEIGHT), 1);
		}
		
		Pathfinding.Init();
	}

	public void CreateTile(int _x, int _y)
	{
		Tile tempTile = Instantiate(tilePrefab, mapTransform).GetComponent<Tile>();
		tempTile.x = _x;
		tempTile.y = _y;
		tempTile.transform.localPosition = new Vector3(_x, _y, 0);
		map[_x, _y] = tempTile;
	}

	public Unit CreateUnit(int _x, int _y, int playerIndex)
	{
		if(GetTile(_x, _y).occupant != null)
		{
			return CreateUnit(Random.Range(0, MAP_WIDTH), Random.Range(0, MAP_HEIGHT), playerIndex);
		}
		Unit tempUnit = Instantiate(unitPrefab, mapTransform).GetComponent<Unit>();
		tempUnit.player = playerIndex;
		tempUnit.SetTile(GetTile(_x, _y));
		return tempUnit;
	}

	public Tile GetTile(int _x, int _y)
	{
		if(_x < 0 || _y < 0 || _x >= MAP_WIDTH || _y >= MAP_HEIGHT)
		{
			print("Out of range!");
			return map[0, 0];
		}
		else
		{
			return map[_x, _y];
		}
	}

	public void EndTurn()
	{
		currentPlayer++;
		if (currentPlayer >= players.Length)
		{
			currentPlayer = 0;
		}
		turnDisplay.text = "PLAYER " + CastleTools.NumberWords(currentPlayer + 1);
		endTurn.Dispatch();
	}

	public Tile GetTile(Pathfinding.PathNode pathNode)
	{
		return GetTile(pathNode.x, pathNode.y);
	}
	// Update is called once per frame
	void Update ()
	{
		CastleManager.CastleUpdate();
		if(state == GameState.IDLE)
		{
			gameUI.alpha = Mathf.Lerp(gameUI.alpha, 1, Time.deltaTime * 10);
			initMap.alpha = Mathf.Lerp(initMap.alpha, 0, Time.deltaTime * 10);
		}
	}
}
