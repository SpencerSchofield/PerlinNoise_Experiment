using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
	public const float maxViewDst = 600;
	public Transform viewer;
	public Material mapMeterial;
	public static Vector2 viewerPosition;
	static MapGenerator mapGenerator;
	int chunkSize;
	int chunkVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> terrainChunkVisibleLastUpdate = new List<TerrainChunk>();

	void Start()
	{
		mapGenerator = FindObjectOfType<MapGenerator>();
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
	}

	void Update()
	{
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		UpdateVisibleChunks();
	}

	void UpdateVisibleChunks()
	{

		for (int i = 0; i < terrainChunkVisibleLastUpdate.Count; i++)
		{
			terrainChunkVisibleLastUpdate[i].SetVisible(false);
		}
		terrainChunkVisibleLastUpdate.Clear();

		int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

		for (int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				//checks if chunk is already loaded by checking the dictionary if its in there, else create new terrain chuck
				if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
				{
					terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
					{
						terrainChunkVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
					}
				}
				else
				{
					terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMeterial));
				}
			}
		}

	}

	public class TerrainChunk
	{
		GameObject meshObject;
		Vector2 position;
		Bounds bounds;
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;

		public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
		{
			position = coord * size;
			bounds = new Bounds(position, Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x, 0, position.y);


			meshObject = new GameObject("Terrain Chunk");
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
		    meshRenderer.material = material;

			meshObject.transform.position = positionV3;
			meshObject.transform.parent = parent;
			SetVisible(false);

			mapGenerator.RequestMapData(OnMapDataRecieved);
		}

		void OnMapDataRecieved(MapData mapdata)
		{
			mapGenerator.RequestMeshData(mapdata, OnMeshDataRecieved);
		}

		void OnMeshDataRecieved(MeshData meshData)
		{
			meshFilter.mesh = meshData.CreateMesh();
		}

		public void UpdateTerrainChunk()
		{
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			bool visible = viewerDstFromNearestEdge <= maxViewDst;
			SetVisible(visible);
		}

		public void SetVisible(bool visible)
		{
			meshObject.SetActive(visible);
		}

		public bool IsVisible()
		{
			return meshObject.activeSelf;
		}

	}

}
