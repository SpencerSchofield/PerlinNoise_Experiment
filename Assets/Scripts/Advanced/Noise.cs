using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
	/*
	GenerateNoiseMap() creates points in a 2D float array using perlin noise which can be used on a plane to create landmasses.
	The seed, noiseScale, octaves, persistance, lacunarity and offset are all used to modify and add additional noise to the map.
	*/
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		System.Random prng = new System.Random(seed);
		Vector2[] octavesOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++)
		{
			float offSetX = prng.Next(-10000,10000) + offset.x;
			float offSetY = prng.Next(-10000,10000) + offset.y;
			octavesOffsets[i] = new Vector2(offSetX,offSetY);
		}
		
		if (scale <= 0) { scale = 0.0001f; }

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;
		
		float halfWidth = mapWidth/2f;
		float halfHeight = mapHeight/2f;
		
		float[,] noiseMap = new float[mapWidth, mapHeight];
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;
				
				for (int i = 0; i < octaves; i++)
				{
					float sampleX = (x- halfWidth) / scale * frequency + octavesOffsets[i].x;
					float sampleY = (y - halfHeight) / scale * frequency + octavesOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;
					
					amplitude *= persistance;
					frequency *= lacunarity;
				}
				if(noiseHeight > maxNoiseHeight)
				{
					maxNoiseHeight = noiseHeight;
				} else if(noiseHeight < minNoiseHeight){
					minNoiseHeight = noiseHeight;
				}
				
				noiseMap[x,y] = noiseHeight;

			}
		}
		
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
			}
		}
		return noiseMap;
	}


}
