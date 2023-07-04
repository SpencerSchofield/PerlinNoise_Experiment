using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode { NoiseMap, ColourMap, Mesh };
	public DrawMode drawMode;
	public const int mapChunkSize = 241;
	[Range(0, 6)]
	public int levelOfDetail;
	public float noiseScale;
	public int octaves;
	[Range(0, 1)]
	public float persistance;
	public float lacunarity;
	public int seed;
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public Vector2 offset;
	public bool autoUpdate;

	public TerrainType[] regions;

	ConcurrentQueue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new ConcurrentQueue<MapThreadInfo<MapData>>();
	ConcurrentQueue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new ConcurrentQueue<MapThreadInfo<MeshData>>();
	
	// Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
	// Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
	public void DrawMapInEditor()
	{
		MapData mapData = GenerateMapData();

		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
		}
		else if (drawMode == DrawMode.ColourMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			display.DrawMesh(MeshGeneratorNew.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
		}
	}

	public void RequestMapData(Action<MapData> callback)
	{
		ThreadStart threadStart = delegate
		{
			MapDataThread(callback);
		};
		new Thread(threadStart).Start();
	}

	void MapDataThread(Action<MapData> callback)
	{
		MapData mapData = GenerateMapData();
		mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
	}
	
	public void RequestMeshData(MapData mapData, Action<MeshData> callback)
	{
		ThreadStart threadStart = delegate
		{
			MeshDataThread(mapData, callback);
		};
		
		new Thread (threadStart).Start();
	}
	void MeshDataThread(MapData mapData, Action<MeshData> callback)
	{
		MeshData meshData = MeshGeneratorNew.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
		meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
		
	}

void Update()
{
	ProcessMapDataThreadQueue();
	ProcessMeshDataThreadQueue();
}

void ProcessMapDataThreadQueue()
{
	while (mapDataThreadInfoQueue.Count > 0)
	{
		if (mapDataThreadInfoQueue.TryDequeue(out MapThreadInfo<MapData> threadInfo))
		{
			threadInfo.callback(threadInfo.parameter);
		}
	}
}

void ProcessMeshDataThreadQueue()
{
	while (meshDataThreadInfoQueue.Count > 0)
	{
		if (meshDataThreadInfoQueue.TryDequeue(out MapThreadInfo<MeshData> threadInfo))
		{
			threadInfo.callback(threadInfo.parameter);
		}
	}
}

	/*
	GenerateMap() is used to generate a new map using perlin noise from the Noise class.
	*/
	MapData GenerateMapData()
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

		for (int y = 0; y < mapChunkSize; y++)
		{
			for (int x = 0; x < mapChunkSize; x++)
			{
				float currentheight = noiseMap[x, y];
				for (int i = 0; i < regions.Length; i++)
				{
					if (currentheight <= regions[i].height)
					{
						colourMap[y * mapChunkSize + x] = regions[i].colour;
						break;
					}
				}
			}
		}

		return new MapData(noiseMap, colourMap);

	}

	void OnValidate()
	{
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}
		if (octaves < 1)
		{
			octaves = 1;
		}

	}

	struct MapThreadInfo<T>
	{
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo(Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}

	}
}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color colour;
}

public struct MapData
{
	public readonly float[,] heightMap;
	public readonly Color[] colourMap;

	public MapData(float[,] heightMap, Color[] colourMap)
	{
		this.heightMap = heightMap;
		this.colourMap = colourMap;
	}
	
}
