using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator : IHeightmapGenerator
{
	public struct singleNoiseMap {
		public NoiseMapGenerator noiseMap;
		public float scalar;
		public singleNoiseMap(NoiseMapGenerator noiseMap, float scalar)
		{
			this.noiseMap = noiseMap;
			this.scalar = scalar;
		}
	}

	List<singleNoiseMap> maps;

	public WorldGenerator(long seed)
	{
		maps = new List<singleNoiseMap>();

		if (seed == 0)
		{
			seed = System.DateTime.Now.Ticks;
		}
		System.Random randomGenerator = new System.Random((int)seed);
		float offsetX = randomGenerator.Next(-100000, 100000);
		float offsetY = randomGenerator.Next(-100000, 100000);
		Vector2 seedOffset = new Vector2(offsetX, offsetY);
		//maps.Add(new singleNoiseMap(new NoiseMapGenerator(0.0005f, seedOffset), 80)); //TODO first fix bug
		maps.Add(new singleNoiseMap(new NoiseMapGenerator(0.002f, seedOffset), 20));
		maps.Add(new singleNoiseMap(new NoiseMapGenerator(0.008f, seedOffset), 5f));
		maps.Add(new singleNoiseMap(new NoiseMapGenerator(0.04f, seedOffset), 0.8f));
		maps.Add(new singleNoiseMap(new NoiseMapGenerator(0.2f, seedOffset), 0.1f));
	}


	public float getPoint(Vector2 point, Vector2 offset)
	{
		float h = 0;
		foreach (singleNoiseMap map in maps)
		{
			h = h + map.noiseMap.getPoint(point, offset) * map.scalar;
		}
		return h;
	}
}
