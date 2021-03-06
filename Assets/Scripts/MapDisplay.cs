﻿using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {
	public Renderer textureRenderer;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	public void Draw(float[,] noiseMap) {
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength (1);

		Texture2D texture = new Texture2D (width, height);

		Color[] colors = new Color[width * height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				colors  [y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x,y]);
			}
		}
		texture.SetPixels(colors);
		texture.Apply();

		textureRenderer.sharedMaterial.mainTexture = texture;
		textureRenderer.transform.localScale = new Vector3 (width, 1, height);
	
	}

	public void drawMesh(Mesh mesh)
	{
		meshFilter.sharedMesh = mesh;
	}
}
