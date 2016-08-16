using UnityEngine;
using System.Collections;

public class NoiseMapGenerator {
	protected float scale;
	protected Vector2 seedOffset;


	public NoiseMapGenerator(float scale, Vector2 seedOffset) {
		this.seedOffset = seedOffset;
		this.scale = scale;
	}

	public float[,] getNoiseMap(int width, int height, int offsetX, int offsetY) {
		float[,] noiseMap = new float[width, height];

		float centerX = ((float)width) / 2.0f;
		float centerY = ((float)height) / 2.0f;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				float coordX = x + offsetX;
				float coordY = y + offsetY;
				float hexOffset = ((coordY % 2) == 0.0f) ? 0 : 0.5f;
				float sampleX = ((coordX - centerX) + seedOffset.x + hexOffset) * scale;
				float sampleY = ((coordY - centerY) + seedOffset.y) * scale;

				noiseMap [x, y] = Mathf.PerlinNoise (sampleX, sampleY);
			}

		}

		return noiseMap;
	}

}
