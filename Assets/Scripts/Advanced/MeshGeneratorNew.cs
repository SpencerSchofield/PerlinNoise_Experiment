using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGeneratorNew
{
	/*
	GenerateTerrainMesh() takes in a heightMap to produce a mesh that's both effected by perlin noise and a colour map
	*/
	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
	{
		AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		int meshSimpleficationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerLine = (width-1)/meshSimpleficationIncrement + 1;

		MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height; y += meshSimpleficationIncrement)
		{
			for (int x = 0; x < width; x += meshSimpleficationIncrement)
			{
				meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
				meshData.uvs[vertexIndex] = new Vector2((x / (float)(width)), y / (float)(height));


				if (x < width - 1 && y < height - 1)
				{
					meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
					meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex++;
			}
		}
		return meshData;
	}
}

/*
MeshData class to hold mesh data such as vertices, triangles and uvs.
*/
public class MeshData
{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;
	int triangleIndex;
	public MeshData(int meshWidth, int meshHeight)
	{
		vertices = new Vector3[meshWidth * meshHeight];
		triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
		uvs = new Vector2[meshWidth * meshHeight];

	}

	/*
	AddTriangles() method is used to add triangles to vertices to create a sqaure 
	*/
	public void AddTriangle(int a, int b, int c)
	{
		triangles[triangleIndex] = a;
		triangles[triangleIndex + 1] = b;
		triangles[triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	/*
	CreateMesh() method adds all the custom information we made into the mesh to create the terrain
	*/
	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		return mesh;
	}

}