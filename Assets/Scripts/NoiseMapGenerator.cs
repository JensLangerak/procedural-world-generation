using UnityEngine;
using System.Collections;

public class NoiseMapGenerator {
	protected float scale;
	protected Vector2 seedOffset;


	public NoiseMapGenerator(float scale, Vector2 seedOffset) {
		this.seedOffset = seedOffset;
		this.scale = scale;
	}

	public float[,] getNoiseMap(int width, int height, Vector2 offset) {
		float[,] noiseMap = new float[width, height];

		float centerX = ((float)width) / 2.0f;
		float centerY = ((float)height) / 2.0f;

		float h_sqrt_3 = Mathf.Sqrt(3.0f) * 0.5f;

		for (int x = 0; x < width; x++) {
			float hexOffset = 0;
			for (int y = 0; y < height; y++) {
				float sampleX = ( ((float) x) + offset.x - centerX + seedOffset.x + hexOffset) * scale;
				float sampleY = (( ((float)y) - centerY + seedOffset.y) * h_sqrt_3 + offset.y) * scale;

				noiseMap [x, y] = Mathf.PerlinNoise (sampleX, sampleY);
				hexOffset = (hexOffset == 0.5)  ? 0 : 0.5f;	
			}

		}

		return noiseMap;
	}

}
