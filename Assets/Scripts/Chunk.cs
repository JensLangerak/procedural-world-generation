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
	protected bool currentLowResTop;
	protected bool currentLowResRight;
	protected bool currentLowResBottom;
	protected bool currentLowResLeft;

	/**
	 * Create a new chunc a the center coordinates.
	 * 
	 * @param center the center coordinates of the chunk.
	 * @param mapGenerator mapGenerator that must be used to generate this chunk.
	 * @param materail material for this chunk.
	 */
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

	/**
	 * Set the level of detail of this chunk.
	 * 
	 * @param levelOfDetail level of detail of this chunk, 1 hightest 7 lowest, 0 not visible.
	 * @param lowRes... render the edge in one level of detail lower, so if this chunk has levelOfDetail 1 and lowResRight is true, then
	 *  the right edge will be rendered with a levelOfDetail, this will result in a smooth transition between chunks.
	 */
	public void visible(int levelOfDetail, bool lowResTop, bool lowResRight, bool lowResBottom, bool lowResLeft)
	{
		if (levelOfDetail == 0)
		{
			chunk.SetActive(false);
		} else if (currentDetail != levelOfDetail || currentLowResLeft != lowResLeft || currentLowResTop != lowResTop || currentLowResRight != lowResRight || currentLowResBottom != lowResBottom) {
			currentDetail = levelOfDetail;
			currentLowResLeft = lowResLeft;
			currentLowResTop = lowResTop;
			currentLowResRight = lowResRight;
			currentLowResBottom = lowResBottom;

			meshFilter.sharedMesh = MeshGenerator.generateMesh(mapGenerator, size + 1, size + 1, center, levelOfDetail, lowResTop, lowResRight, lowResBottom, lowResLeft);
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
