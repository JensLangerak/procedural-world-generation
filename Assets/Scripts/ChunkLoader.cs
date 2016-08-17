using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkLoader : MonoBehaviour {
	public const int viewDistance = 500;
	public int visibleChunks;
	public Transform player;
	public MapGenerator generator;
	public Material materail;

	protected Vector2 playerPos;
	protected NoiseMapGenerator mapGenerator;

	Dictionary<Vector2, Chunk> chunks = new Dictionary<Vector2, Chunk>();
	

	// Use this for initialization
	void Start () {
		mapGenerator = generator.createGenerator();
		visibleChunks = viewDistance / Chunk.size;
	}
	
	// Update is called once per frame
	void Update () {
		playerPos = new Vector2(player.position.x, player.position.z);
		List<Vector2> removeKeys = new List<Vector2>();
		float h_sqrt_3 = Mathf.Sqrt(3.0f) * 0.5f;

		foreach (KeyValuePair<Vector2, Chunk> chunk in chunks)
		{
			Vector2 center = chunk.Value.getCenter();
			if (center.x < playerPos.x - (1 + visibleChunks) * Chunk.size || center.x > playerPos.x + (1 + visibleChunks) * Chunk.size
				|| center.y < playerPos.y - (1.1 + visibleChunks) * Chunk.size * h_sqrt_3 || center.y > playerPos.y + (1.1 + visibleChunks) * Chunk.size * h_sqrt_3)
			{
				chunk.Value.visible(0);
				removeKeys.Add(chunk.Key);
				Destroy(chunk.Value.getObject());
			}
		}

		foreach(Vector2 key in removeKeys)
		{
			chunks.Remove(key);
		}


		int centerX = (int)playerPos.x / Chunk.size;
		int centerY = (int)playerPos.y / Chunk.size;

		float f_centerX = ((float)centerX) * Chunk.size + 0;
		float f_centerY = ((float)centerY * Chunk.size) * h_sqrt_3;
	
		for (int x = -visibleChunks; x <= visibleChunks; x++)
		{
			for (int y = -visibleChunks; y <= visibleChunks; y++)
			{
				Chunk chunk;
				int details = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) + 1;
				details = (details < 1) ? 1 : details;
				if (chunks.TryGetValue(new Vector2(f_centerX + (x * Chunk.size - 0), f_centerY + y * Chunk.size * h_sqrt_3), out chunk))
				{
					chunk.visible(details);
				} else
				{
					chunk = new Chunk(new Vector2(f_centerX + (x * Chunk.size - 0), f_centerY + y * Chunk.size * h_sqrt_3), mapGenerator, materail);
					chunk.visible(details);
					chunks.Add(new Vector2(f_centerX + (x * Chunk.size - 0), f_centerY + y * Chunk.size * h_sqrt_3), chunk);
				}
			}
		}



	}
}
