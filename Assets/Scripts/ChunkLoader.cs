using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct ChunkCoord
{
	public int coordX;
	public int coordY;
}

public class ChunkLoader : MonoBehaviour {
	public const int viewDistance = 800;
	public int visibleChunks;
	public Transform player;
	public MapGenerator generator;
	public Material materail;

	protected Vector2 playerPos;
	protected NoiseMapGenerator mapGenerator;

	Dictionary<ChunkCoord, Chunk> chunks = new Dictionary<ChunkCoord, Chunk>();
	protected float h_sqrt_3 = Mathf.Sqrt(3.0f) * 0.5f;

	// Use this for initialization
	void Start () {
		mapGenerator = generator.createGenerator();
		visibleChunks = viewDistance / Chunk.size + 1; /* sometimes when the player is near an edge an extra chunk is loaded */
	}
	
	// Update is called once per frame
	void Update () {
		playerPos = new Vector2(player.position.x, player.position.z);
		List<ChunkCoord> removeKeys = new List<ChunkCoord>();

		foreach (KeyValuePair<ChunkCoord, Chunk> chunk in chunks)
		{
			Vector2 center = chunk.Value.getCenter();
			if (distanceChunkToPlayer(center) > viewDistance)
			{
				chunk.Value.visible(0, false, false, false, false);
				removeKeys.Add(chunk.Key);
				Destroy(chunk.Value.getObject());
			}
		}

		foreach(ChunkCoord key in removeKeys)
		{
			chunks.Remove(key);
		}
		

		int centerX = (int)playerPos.x / Chunk.size;
		int centerY = (int)(playerPos.y / (Chunk.size * h_sqrt_3));

		float f_centerX = ((float)centerX) * Chunk.size + 0;
		float f_centerY = ((float)centerY) * Chunk.size * h_sqrt_3;
	
		
		for (int x = -visibleChunks; x <= visibleChunks; x++)
		{
			for (int y = -visibleChunks; y <= visibleChunks; y++)
			{
				
				Vector2 chunkCenter = new Vector2(f_centerX + (x * Chunk.size - 0), f_centerY + y * Chunk.size * h_sqrt_3);
				
				loadChunk(chunkCenter);
			}
		}
	}

	protected void loadChunk(Vector2 chunkCenter)
	{
		Chunk chunk;
		int details = getDetails(chunkCenter);
		ChunkCoord chunkCoord;
		float f_chunkCoordX = chunkCenter.x / (float)Chunk.size;
		float f_chunkCoordY = chunkCenter.y / (((float)Chunk.size) * h_sqrt_3);
		chunkCoord.coordX = (f_chunkCoordX > 0) ? Mathf.FloorToInt(f_chunkCoordX) : Mathf.CeilToInt(f_chunkCoordX);
		chunkCoord.coordY = (f_chunkCoordY > 0) ? Mathf.FloorToInt(f_chunkCoordY) : Mathf.CeilToInt(f_chunkCoordY);

		if (details == 0)
		{
			return;
		}
		if (!chunks.TryGetValue(chunkCoord, out chunk))
		{
			chunk = new Chunk(chunkCenter, mapGenerator, materail);
			chunks.Add(chunkCoord, chunk);
		}
		bool lowResTop = getDetails(new Vector2(chunkCenter.x, chunkCenter.y + Chunk.size * h_sqrt_3)) > details;
		bool lowResBottom = getDetails(new Vector2(chunkCenter.x, chunkCenter.y - Chunk.size * h_sqrt_3)) > details;
		bool lowResLeft = getDetails(new Vector2(chunkCenter.x - Chunk.size, chunkCenter.y)) > details;
		bool lowResRight = getDetails(new Vector2(chunkCenter.x + Chunk.size, chunkCenter.y)) > details;

		chunk.visible(details, lowResTop, lowResRight, lowResBottom, lowResLeft);
	}

	protected float distanceChunkToPlayer(Vector2 chunkCenter)
	{
		float diffX = 0.5f * Chunk.size;
		float closestX = (chunkCenter.x + diffX < playerPos.x) ? chunkCenter.x + diffX :
			(chunkCenter.x - diffX > playerPos.x) ? chunkCenter.x - diffX : playerPos.x;

		float diffY = 0.5f * Chunk.size * h_sqrt_3;
		float closestY = (chunkCenter.y + diffY < playerPos.y) ? chunkCenter.y + diffY :
			(chunkCenter.y - diffY > playerPos.y) ? chunkCenter.y - diffY : playerPos.y;

		return Vector2.Distance(playerPos, new Vector2(closestX, closestY));
	}

	protected int getDetails(Vector2 chunkCenter)
	{
		float distance = distanceChunkToPlayer(chunkCenter);
		float increasedDistance = distance + 3 * Chunk.size / 4;
		int details = (int)increasedDistance / Chunk.size + 1;
		details = (distance > viewDistance) ? 0 : details;
		return details;
	}
}
