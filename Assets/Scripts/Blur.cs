using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blur : MonoBehaviour
{
	public static bool active;
	public Image blurImage;
	
	// Update is called once per frame
	void Update ()
	{
		if(active)
		{
			blurImage.color = Color.Lerp(blurImage.color, Color.black, Time.deltaTime * 5);
		}
		else
		{
			blurImage.color = Color.Lerp(blurImage.color, Color.clear, Time.deltaTime * 5);
		}
	}
}
