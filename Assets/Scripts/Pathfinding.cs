// @uniquecodes
// /u/uniquecornDev

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
	public struct PathNode
	{
		public int x;
		public int y;
		public int index;
		public bool blocked;

		public Vector3 GetPosition()
		{
			return GameManager.instance.GetTile(x,y).transform.position;
		}
	}
	public List<PathNode> path;
	public PathNode[,] pathNodes;
	public Tile start;
	public Tile end;
	private List<PathNode> nodesToSearch;
	private PathNode[] nodeCheck;

	public int pathLength;

	public static PathNode[,] nodeMap;
	/// <summary>
	/// Call this after your map is created. Initiates a standard node map that every path can use.
	/// </summary>
	public static void Init()
	{
		nodeMap = new PathNode[GameManager.MAP_WIDTH, GameManager.MAP_HEIGHT];
		for (int i = 0; i < GameManager.MAP_WIDTH; i++)
		{
			for (int j = 0; j < GameManager.MAP_HEIGHT; j++)
			{
				nodeMap[i, j] = new PathNode()
				{
					x = i,
					y = j,
					index = -1
				};
			}
		}
	}
	/// <summary>
	/// Creates a pathfinding instance and generates a path for you.
	/// </summary>
	/// <param name="start">Start tile</param>
	/// <param name="end">End tile</param>
	/// <returns></returns>
	public static Pathfinding GetPath(Tile start, Tile end)
	{
		Pathfinding newPath = new Pathfinding();

		newPath.Create(start, end);
		return newPath;
	}
	/// <summary>
	/// Creates a pathfinding instance and generates a range map for you.
	/// </summary>
	/// <param name="start">Start tile</param>
	/// <param name="range">Range of tiles</param>
	/// <returns></returns>
	public static Pathfinding GetRange(Tile start,int range, bool ignoreBlock = false)
	{
		Pathfinding newPath = new Pathfinding();

		newPath.CreateRangedMap(start,range,ignoreBlock);
		return newPath;
	}

	/// <summary>
	/// Creates the path from tile A to tile B
	/// </summary>
	/// <param name="a">First tile</param>
	/// <param name="b">Second tile</param>
	public void Create(Tile a, Tile b)
	{
		if (a == null || b == null || b.blocked || a.blocked)
		{
			return;
		}

		pathNodes = (PathNode[,])nodeMap.Clone();
		GenerateNodeMap();

		start = a;
		end = b;
		pathNodes[start.x, start.y].index = 0;

		if (nodesToSearch != null)
		{
			nodesToSearch.Clear();
		}
		else
		{
			nodesToSearch = new List<PathNode>();
		}

		bool pathFound = Iterate(pathNodes[a.x, a.y], nodesToSearch);

		while (!pathFound)
		{
			if (nodesToSearch.Count >= 1)
			{
				pathFound = Search();
			}
			else
			{
				break;
			}
		}

		if (pathFound)
		{
			FindPath();
		}
		else
		{
			Debug.Log("NO PATH WTF ???");
		}
	}
	/// <summary>
	/// Checks the range from the starting tile outwards
	/// </summary>
	/// <param name="start">First tile</param>
	/// <param name="range">Range</param>
	public void CreateRangedMap(Tile start, int range = 2, bool ignoreBlock = false)
	{
		if (start == null)
		{
			return;
		}

		pathNodes = (PathNode[,])nodeMap.Clone();
		GenerateNodeMap(ignoreBlock);

		pathNodes[start.x, start.y].index = 0;

		if (nodesToSearch != null)
		{
			nodesToSearch.Clear();
		}
		else
		{
			nodesToSearch = new List<PathNode>();
		}
		int minRange = RangeIterate(pathNodes[start.x, start.y], nodesToSearch);

		while(minRange < range)
		{
			if (nodesToSearch.Count > 1)
			{
				minRange = RangeSearch();
			}
			else
			{
				break;
			}
		}
		path = new List<PathNode>();
		for (int i = 0; i < GameManager.MAP_WIDTH; i++)
		{
			for (int j = 0; j < GameManager.MAP_HEIGHT; j++)
			{
				if (pathNodes[i, j].index <= range && pathNodes[i,j].index != -1)
				{
					path.Add(pathNodes[i, j]);
				}
			}
		}
	}
	/// <summary>
	/// Iterates nodes outwards toward tile B
	/// </summary>
	/// <returns></returns>
	bool Search()
	{
		nodeCheck = new PathNode[nodesToSearch.Count];
		nodesToSearch.CopyTo(nodeCheck);
		nodesToSearch.Clear();
		foreach (PathNode _node in nodeCheck)
		{
			if (Iterate(_node, nodesToSearch))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Iterates nodes outwards for range checking
	/// </summary>
	/// <returns></returns>
	int RangeSearch()
	{
		nodeCheck = new PathNode[nodesToSearch.Count];
		nodesToSearch.CopyTo(nodeCheck);
		nodesToSearch.Clear();
		int minRange = 99;
		int tempMinRange;
		foreach (PathNode _node in nodeCheck)
		{
			tempMinRange = RangeIterate(_node, nodesToSearch);
			if(tempMinRange < minRange)
			{
				minRange = tempMinRange;
			}
		}
		return minRange;
	}

	/// <summary>
	/// Uses the nodemap to find the quickest path back to tile A.
	/// </summary>
	void FindPath()
	{
		PathNode currentNode = pathNodes[end.x, end.y];
		path = new List<PathNode>
		{
			currentNode
		};
		int mapSize = GameManager.MAP_WIDTH * GameManager.MAP_HEIGHT;
		int i = 0;
		while(i < mapSize)
		{
			int indexChosen = 0;
			int indexDifference = 0;

			int xDiff = 0;
			int yDiff = 0;

			for(int j = 0; j < 8; j++)
			{
				switch(j)
				{
					case 0:
						xDiff = currentNode.x + 1;
						yDiff = currentNode.y;
						break;
					case 1:
						xDiff = currentNode.x - 1;
						yDiff = currentNode.y;
						break;
					case 2:
						xDiff = currentNode.x;
						yDiff = currentNode.y + 1;
						break;
					case 3:
						xDiff = currentNode.x;
						yDiff = currentNode.y - 1;
						break;
					case 4:
						xDiff = currentNode.x - 1;
						yDiff = currentNode.y + 1;
						break;
					case 5:
						xDiff = currentNode.x - 1;
						yDiff = currentNode.y - 1;
						break;
					case 6:
						xDiff = currentNode.x + 1;
						yDiff = currentNode.y + 1;
						break;
					case 7:
						xDiff = currentNode.x + 1;
						yDiff = currentNode.y - 1;
						break;
				}
				if(xDiff == start.x && yDiff == start.y)
				{
					indexChosen = j;
					indexDifference = 1;
					break;
				}
				else if(xDiff < 0 ||  xDiff >= GameManager.MAP_WIDTH || yDiff < 0 || yDiff >= GameManager.MAP_HEIGHT)
				{

				}
				else
				{
					if(pathNodes[xDiff,yDiff].index != -1 && pathNodes[xDiff,yDiff].index != 9999)
					{
						if(currentNode.index - pathNodes[xDiff, yDiff].index > indexDifference)
						{
							indexChosen = j;
							indexDifference = currentNode.index - pathNodes[xDiff, yDiff].index;
						}
					}
				}
			}
			if(indexDifference > 0)
			{
				switch (indexChosen)
				{
					case 0:
						xDiff = currentNode.x + 1;
						yDiff = currentNode.y;
						break;
					case 1:
						xDiff = currentNode.x - 1;
						yDiff = currentNode.y;
						break;
					case 2:
						xDiff = currentNode.x;
						yDiff = currentNode.y + 1;
						break;
					case 3:
						xDiff = currentNode.x;
						yDiff = currentNode.y - 1;
						break;
					case 4:
						xDiff = currentNode.x - 1;
						yDiff = currentNode.y + 1;
						break;
					case 5:
						xDiff = currentNode.x - 1;
						yDiff = currentNode.y - 1;
						break;
					case 6:
						xDiff = currentNode.x + 1;
						yDiff = currentNode.y + 1;
						break;
					case 7:
						xDiff = currentNode.x + 1;
						yDiff = currentNode.y - 1;
						break;
				}
				currentNode = pathNodes[xDiff, yDiff];
				path.Add(currentNode);
				i++;
			}
			else
			{
				i = mapSize;
			}
		}
		path.Reverse();
	}
	/// <summary>
	/// Iterates through the nodeList checking around them and filling up the nodemap values for paths.
	/// </summary>
	/// <param name="_node">Node to check</param>
	/// <param name="nodeList">Array of unchecked nodes to add to your check</param>
	/// <returns>Returns true if it reached the end tile.</returns>
	bool Iterate(PathNode _node, List<PathNode> nodeList)
	{
		int xDiff = 0;
		int yDiff = 0;
		int indexLength = 0;
		for (int j = 0; j < 8; j++)
		{
			switch (j)
			{
				case 0:
					xDiff = _node.x + 1;
					yDiff = _node.y;
					indexLength = 2;
					break;
				case 1:
					xDiff = _node.x - 1;
					yDiff = _node.y;
					indexLength = 2;
					break;
				case 2:
					xDiff = _node.x;
					yDiff = _node.y + 1;
					indexLength = 2;
					break;
				case 3:
					xDiff = _node.x;
					yDiff = _node.y - 1;
					indexLength = 2;
					break;
				case 4:
					xDiff = _node.x - 1;
					yDiff = _node.y + 1;
					indexLength = 3;
					break;
				case 5:
					xDiff = _node.x - 1;
					yDiff = _node.y - 1;
					indexLength = 3;
					break;
				case 6:
					xDiff = _node.x + 1;
					yDiff = _node.y + 1;
					indexLength = 3;
					break;
				case 7:
					xDiff = _node.x + 1;
					yDiff = _node.y - 1;
					indexLength = 3;
					break;
			}
			if (xDiff == end.x && yDiff == end.y)
			{
				pathNodes[xDiff, yDiff].index = _node.index + indexLength;
				nodeList.Add(pathNodes[xDiff, yDiff]);
				return true;
			}
			else if (xDiff < 0 || xDiff >= GameManager.MAP_WIDTH || yDiff < 0 || yDiff >= GameManager.MAP_HEIGHT)
			{

			}
			else if (pathNodes[xDiff, yDiff].index == -1)
			{
				pathNodes[xDiff, yDiff].index = _node.index + indexLength;
				nodeList.Add(pathNodes[xDiff, yDiff]);
			}
		}
		return false;
	}
	/// <summary>
	/// Iterates through the nodeList checking around them and filling up the nodemap values for range checks.
	/// </summary>
	/// <param name="_node">Node to check</param>
	/// <param name="nodeList">Array of unchecked nodes to add to your check</param>
	/// <returns>Returns the lowest</returns>
	int RangeIterate(PathNode _node, List<PathNode> nodeList)
	{
		int xDiff = 0;
		int yDiff = 0;
		int indexLength = 0;
		int minRange = 99;
		for (int j = 0; j < 8; j++)
		{
			switch (j)
			{
				case 0:
					xDiff = _node.x + 1;
					yDiff = _node.y;
					indexLength = 2;
					break;
				case 1:
					xDiff = _node.x - 1;
					yDiff = _node.y;
					indexLength = 2;
					break;
				case 2:
					xDiff = _node.x;
					yDiff = _node.y + 1;
					indexLength = 2;
					break;
				case 3:
					xDiff = _node.x;
					yDiff = _node.y - 1;
					indexLength = 2;
					break;
				case 4:
					xDiff = _node.x - 1;
					yDiff = _node.y + 1;
					indexLength = 3;
					break;
				case 5:
					xDiff = _node.x - 1;
					yDiff = _node.y - 1;
					indexLength = 3;
					break;
				case 6:
					xDiff = _node.x + 1;
					yDiff = _node.y + 1;
					indexLength = 3;
					break;
				case 7:
					xDiff = _node.x + 1;
					yDiff = _node.y - 1;
					indexLength = 3;
					break;
			}
			if (xDiff < 0 || xDiff >= GameManager.MAP_WIDTH || yDiff < 0 || yDiff >= GameManager.MAP_HEIGHT)
			{

			}
			else if (pathNodes[xDiff, yDiff].index == -1)
			{
				pathNodes[xDiff, yDiff].index = _node.index + indexLength;
				if(minRange > pathNodes[xDiff, yDiff].index)
				{
					minRange = pathNodes[xDiff, yDiff].index;
				}
				nodeList.Add(pathNodes[xDiff, yDiff]);
			}
		}
		return minRange;
	}
	/// <summary>
	/// Generates the nodemap.
	/// </summary>
	void GenerateNodeMap(bool ignoreBlock = false)
	{
		for (int i = 0; i < GameManager.MAP_WIDTH; i++)
		{
			for (int j = 0; j < GameManager.MAP_HEIGHT; j++)
			{
				Tile tempTile = GameManager.instance.GetTile(i, j);
				
				if(tempTile.occupant && !ignoreBlock)
				{
					pathNodes[i, j].blocked = true;
					pathNodes[i, j].index = 9999;
				}
				else
				{
					pathNodes[i, j].index = -1;
				}
			}
		}
	}
}
