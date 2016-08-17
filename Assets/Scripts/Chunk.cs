using UnityEngine;
using System.Collections;

public class Chunk  {
	public static int size = 128;
	protected Vector2 center;

	protected GameObject chunk;
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;

	protected float[,] map;
	protected int currentDetail;

	public Chunk(Vector2 center, NoiseMapGenerator mapGenerator, Material materail)
	{
		this.center = center;
		map = mapGenerator.getNoiseMap(size + 1, size + 1, center);

		chunk = new GameObject("Chunk");
		meshRenderer = chunk.AddComponent<MeshRenderer>();
		meshFilter = chunk.AddComponent<MeshFilter>();
		meshRenderer.material = materail;

		chunk.transform.position = new Vector3(center.x, 0, center.y);
		chunk.SetActive(false);
		currentDetail = 0;
	}

	public void visible(int levelOfDetail)
	{
		if (levelOfDetail == 0)
		{
			chunk.SetActive(false);
		} else if (currentDetail != levelOfDetail) { 
			meshFilter.sharedMesh = MeshGenerator.generateMesh(map, false, levelOfDetail);
			currentDetail = levelOfDetail;
			chunk.SetActive(true);
			
		}
	}

	public Vector2 getCenter()
	{
		return center;
	}

	public GameObject getObject()
	{
		return chunk;
	}

}
