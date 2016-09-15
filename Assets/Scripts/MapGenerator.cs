using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {
	public enum DrawMode {NoiseMap, Mesh}
	public DrawMode drawMode;

	public long seed;
	public int mapWith;
	public int mapHeight;
	public float scale;
	NoiseMapGenerator noiseMapGenerator;
	public bool autoUpdate;
	public Vector2 pos;

	[Range(1,7)]
	public int detail = 1;

	public NoiseMapGenerator createGenerator()
	{
		if (seed == 0)
		{
			seed = System.DateTime.Now.Ticks;
		}
		System.Random randomGenerator = new System.Random((int)seed);

		float offsetX = randomGenerator.Next(-100000, 100000);
		float offsetY = randomGenerator.Next(-100000, 100000);
		Vector2 seedOffset = new Vector2(offsetX, offsetY);

		return new NoiseMapGenerator(scale, seedOffset);
	}

	public void generateMap() {
		noiseMapGenerator = createGenerator();

		float[,] map = noiseMapGenerator.getNoiseMap (mapWith, mapHeight, pos);

		MapDisplay display = FindObjectOfType<MapDisplay> ();

		if (drawMode == DrawMode.NoiseMap)
		{
			display.Draw(map);
		} else if (drawMode == DrawMode.Mesh) {
			display.drawMesh(MeshGenerator.generateMesh(noiseMapGenerator, mapWith, mapHeight, pos, detail, false, false, false, false));
		}

	}
}
