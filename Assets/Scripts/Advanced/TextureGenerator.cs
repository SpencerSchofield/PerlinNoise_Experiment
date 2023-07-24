using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
	/*
	TextureFromColourMap() returns a colour map of perlin noise, the colours and height values can be edited using the "region" in the inspector
	*/
	public static Texture2D TextureFromColourMap(Color[] colorMap, int width, int height){
		Texture2D texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colorMap);
		texture.Apply();
		return texture;
	}
	/*
	TextureFromHeightMap() returns a greyscale image of perlin noise in its basic form
	*/
	public static Texture2D TextureFromHeightMap(float[,] heightMap)
	{
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
			}
		}
		return TextureFromColourMap(colourMap, width, height);
	}
	
}
