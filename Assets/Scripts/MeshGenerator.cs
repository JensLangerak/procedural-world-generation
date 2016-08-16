using UnityEngine;
using System.Collections;

public class MeshGenerator {

	public static Mesh generateMesh(float[,] map, bool indent)
	{
		int width = map.GetLength(0);
		int height = map.GetLength(1);
		float centerX = ((float)width) / 2.0f;
		float centerY = ((float)height) / 2.0f;

		Vector3[] vertices = new Vector3[width * height];
		int[] triangles = new int[(width - 1) * (height - 1) * 2 * 3];
		Vector2[] uv = new Vector2[width * height];
		int verticeIndex = 0;
		int triangleIndex = 0;

		float h_sqrt_3 = Mathf.Sqrt(3.0f) * 0.5f;
		for (int y = 0; y < height; y++)
		{
			float offset = (indent) ? 0.5f : 0f;
			for (int x = 0; x < width; x++)
			{
			
				
				vertices[verticeIndex] = new Vector3(((float) x) + offset - centerX, map[x,y] * 10, ((float)y)* h_sqrt_3 - centerY);
				
				if (y > 0)
				{
					if (indent)
					{
						if (x >= 1)
						{
							triangles[triangleIndex] = verticeIndex - width;
							triangles[triangleIndex + 1] = verticeIndex - 1;
							triangles[triangleIndex + 2] = verticeIndex;
							triangleIndex += 3;
						}
						if (x < width - 1)
						{
							triangles[triangleIndex] = verticeIndex - width + 1;
							triangles[triangleIndex + 1] = verticeIndex - width;
							triangles[triangleIndex + 2] = verticeIndex;
							triangleIndex += 3;
						}
					} else {
						if (x >= 1)
						{
							
							triangles[triangleIndex] = verticeIndex - width - 1;
							triangles[triangleIndex + 1] = verticeIndex - 1;
							triangles[triangleIndex + 2] = verticeIndex;
							triangleIndex += 3;
							
							triangles[triangleIndex] = verticeIndex - width ;
							triangles[triangleIndex + 1] = verticeIndex - width - 1;
							triangles[triangleIndex + 2] = verticeIndex;
							triangleIndex += 3;
						}
					}

				}
				uv[verticeIndex] = new Vector2(x / (float)width, y / (float)height);

				verticeIndex++;
			}
			indent = !indent;
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.RecalculateNormals();
		return mesh;
	}
}
