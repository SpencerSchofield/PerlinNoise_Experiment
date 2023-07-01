using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This method can be attached to an object with a terrain component, its main function is to produce a random landmass using perlin noise and Unity's inbuilt terrain engine 
Terrain engine is easy to implement but is fairly old and not optimised 
*/
public class TerrainGeneration : MonoBehaviour
{
	public int depth = 20; //height
	public int width = 256; //x
	public int height = 256; //y
	public float scale = 20;
	
	public float offsetX = 100;
	public float offsetY = 100;
	
	void Start()
	{
		offsetX = Random.Range(0f,9999f);
		offsetY = Random.Range(0f,9999f);
	}
	
	/*
	We directly access the terrain data by gettings its component and keep it in an update so I can make real time edits to the values - The below Update() should be in Start()
	if used within a real project
	*/
	void Update()
	{
		Terrain terrain = GetComponent<Terrain>();
		terrain.terrainData = GeenerateTerrain(terrain.terrainData);

	}
	TerrainData GeenerateTerrain(TerrainData terrainData)
	{
		terrainData.heightmapResolution = width;
		terrainData.size = new Vector3(width, depth, height);
		terrainData.SetHeights(0, 0, GenerateHeights());

		return terrainData;
	}

	float[,] GenerateHeights()
	{
		float[,] heights = new float[width, height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				heights[x, y] = CalculateHeight(x, y);
			}
		}
		return heights;
	}

	float CalculateHeight(int x, int y)
	{
		float xCoord = (float)x / width * scale + offsetX;
		float yCoord = (float)y / height * scale + offsetY;
		
		return Mathf.PerlinNoise(xCoord, yCoord);
	}
}
