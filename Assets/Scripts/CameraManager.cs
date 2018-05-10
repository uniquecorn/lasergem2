using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class CameraManager : MonoBehaviour
{
	private Vector2 camFocus;
	public float camDistance;
	public float camDrag;
	// Use this for initialization
	void Start ()
	{
		
	}

	public void Focus(Vector2 pos)
	{
		camFocus = pos;
	}

	public void Focus(CastleObject obj)
	{
		Focus(obj.transform.position);
	}

	public void Snap(float _x, float _y)
	{
		Snap(new Vector2(_x, _y));
	}

	public void Snap(Vector2 pos)
	{
		camFocus = pos;
		Snap();
	}

	public void Snap()
	{
		transform.position = new Vector3(camFocus.x, camFocus.y, -camDistance);
	}

	// Update is called once per frame
	void Update ()
	{
		transform.position = Vector3.Lerp(transform.position, new Vector3(camFocus.x, camFocus.y, -camDistance), Time.deltaTime * camDrag);	
	}
}
