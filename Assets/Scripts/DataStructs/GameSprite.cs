using System.IO;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameSprite
{
	public string spriteID;
	public string spritePath;
	public bool Animated
	{
		get
		{
			if(art.Length <= 1)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
	public SpriteBounds[] art;
	[System.Serializable]
	public class SpriteBounds
	{
		public float x, y, width, height;
		[System.NonSerialized]
		public Sprite sprite;
		public SpriteBounds(float _x, float _y, float _width, float _height)
		{
			x = _x;
			y = _y;
			width = _width;
			height = _height;
		}
		public Rect GetRect()
		{
			return new Rect(x, y, width, height);
		}
		public void Init(Texture2D tex)
		{
			sprite = Sprite.Create(tex, GetRect(), Vector2.one/2, 250);
		}
	}
	public void Init()
	{
		Texture2D artTex = null;
		
		artTex = CastleTools.LoadImage(GetArtPath());
		if(art == null || art.Length == 0)
		{
			art = new SpriteBounds[1]
			{
				new SpriteBounds(0,0,artTex.width,artTex.height)
			};
		}
		for (int i = 0; i < art.Length; i++)
		{
			art[i].Init(artTex);
		}
	}
	public string GetArtPath()
	{
		return Path.Combine(SaveManager.GetCurrentPath(), spritePath);
	}
}
