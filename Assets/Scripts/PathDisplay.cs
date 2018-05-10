using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDisplay : MonoBehaviour
{
	public LineRenderer line;
	public bool hidden = true;
	// Use this for initialization
	public void Display (Pathfinding pathModule)
	{
		hidden = false;
		line.positionCount = pathModule.path.Count;
		for (int i = 0; i < pathModule.path.Count; i++)
		{
			line.SetPosition(i, pathModule.path[i].GetPosition());
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(hidden)
		{
			line.startColor = line.endColor = Color.Lerp(line.startColor, new Color(line.startColor.r, line.startColor.g, line.startColor.b, 0), Time.deltaTime * 5);
		}
		else
		{
			line.startColor = line.endColor = Color.Lerp(line.startColor, new Color(line.startColor.r, line.startColor.g, line.startColor.b, 1), Time.deltaTime * 5);
		}
	}
}
