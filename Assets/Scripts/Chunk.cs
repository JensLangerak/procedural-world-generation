using UnityEngine;
using System.Collections;

public class Chunk  {
	public static int size = 128;
	protected Vector2 center;

	protected GameObject chunk;
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	NoiseMapGenerator mapGenerator;

	protected int currentDetail;

	public Chunk(Vector2 center, NoiseMapGenerator mapGenerator, Material materail)
	{
		this.center = center;
	
		chunk = new GameObject("Chunk");
		meshRenderer = chunk.AddComponent<MeshRenderer>();
		meshFilter = chunk.AddComponent<MeshFilter>();
		meshRenderer.material = materail;
		this.mapGenerator = mapGenerator;

		chunk.transform.position = new Vector3(center.x, 0, center.y);
		chunk.SetActive(false);
		currentDetail = 0;
	}

	public void visible(int levelOfDetail, bool lowResTop, bool lowResRight, bool lowResBottom, bool lowResLeft)
	{
		if (levelOfDetail == 0)
		{
			chunk.SetActive(false);
		} else if (currentDetail != levelOfDetail) { 
			meshFilter.sharedMesh = MeshGenerator.generateMesh(mapGenerator, size, size, center, levelOfDetail, lowResTop, lowResRight, lowResBottom, lowResLeft);
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
