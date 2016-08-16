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
	public int coordX;
	public int coordY;
	public bool autoUpdate;

	public void generateMap() {
		if (seed == 0) {
			seed = System.DateTime.Now.Ticks;
		}
		System.Random randomGenerator = new System.Random((int) seed);

		float offsetX = randomGenerator.Next (-100000, 100000);
		float offsetY = randomGenerator.Next (-100000, 100000);
		Vector2 seedOffset = new Vector2 (offsetX, offsetY);
		
		noiseMapGenerator = new NoiseMapGenerator (scale, seedOffset);
		float[,] map = noiseMapGenerator.getNoiseMap (mapWith, mapHeight, coordX, coordY);

		MapDisplay display = FindObjectOfType<MapDisplay> ();

		if (drawMode == DrawMode.NoiseMap)
		{
			display.Draw(map);
		} else if (drawMode == DrawMode.Mesh) {
			display.drawMesh(MeshGenerator.generateMesh(map, ((coordY % 2) == 1)));
		}

	}
}
