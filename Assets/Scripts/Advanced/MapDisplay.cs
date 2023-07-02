using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
	public Renderer textureRenderer;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	
	/*
	DrawTexture() takes in a texture2D and draws its on a plane
	*/
	public void DrawTexture(Texture2D texture)
	{
		textureRenderer.sharedMaterial.mainTexture = texture;
		textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}
	
	/*
	DrawMesh() takes in mesh data and a texture2D to create a terrain with custom mesh and textures
	*/
	public void DrawMesh(MeshData meshData, Texture2D texture){
		meshFilter.sharedMesh = meshData.CreateMesh();
		meshRenderer.sharedMaterial.mainTexture = texture;
	}

}
