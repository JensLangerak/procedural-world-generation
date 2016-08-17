using UnityEngine;
using System.Collections;

public class MeshGenerator {

	public static Mesh generateMesh(float[,] map, bool indent, Vector2 center, int detail)
	{
		detail = (detail > 7) ? 7 : (detail < 1) ? 1 : detail;
		detail = detail - 1;

		int detailSkippedPoints = (int) Mathf.Pow(2, detail);

		
		int width = map.GetLength(0);
		int height = map.GetLength(1);
		float centerX = center.x - ((float)width) / 2.0f;
		float centerY = center.y - ((float)height) / 2.0f;

		int indexWidth = (width - 1) / detailSkippedPoints + 1;
		int indexHeight = (height - 1) / detailSkippedPoints + 1;

		Vector3[] vertices = new Vector3[indexWidth * indexHeight];
		int[] triangles = new int[(indexWidth - 1) * (indexHeight - 1) * 2 * 3];
		Vector2[] uv = new Vector2[indexWidth * indexHeight];
		int verticeIndex = 0;
		int triangleIndex = 0;

		float h_sqrt_3 = Mathf.Sqrt(3.0f) * 0.5f;
		for (int y = 0; y < height; y++)
		{
			if (y % detailSkippedPoints != 0)
			{
				continue;
			}
			float offset = (indent) ? 0.5f * detailSkippedPoints : 0f;
			for (int x = 0; x < width; x++)
			{
				if (x % detailSkippedPoints != 0)
				{
					continue;
				}

				vertices[verticeIndex] = new Vector3(((float) x) + offset + centerX, map[x,y] * 10, ((float)y)* h_sqrt_3 + centerY);
				
				if (y > 0)
				{
					if (indent)
					{
						if (x >= 1)
						{
							triangles[triangleIndex] = verticeIndex - indexWidth;
							triangles[triangleIndex + 1] = verticeIndex - 1;
							triangles[triangleIndex + 2] = verticeIndex;
							triangleIndex += 3;
						}
						if (x < width - 1)
						{
							triangles[triangleIndex] = verticeIndex - indexWidth + 1;
							triangles[triangleIndex + 1] = verticeIndex - indexWidth;
							triangles[triangleIndex + 2] = verticeIndex;
							triangleIndex += 3;
						}
					} else {
						if (x >= 1)
						{
							
							triangles[triangleIndex] = verticeIndex - indexWidth - 1;
							triangles[triangleIndex + 1] = verticeIndex - 1;
							triangles[triangleIndex + 2] = verticeIndex;
							triangleIndex += 3;
							
							triangles[triangleIndex] = verticeIndex - indexWidth;
							triangles[triangleIndex + 1] = verticeIndex - indexWidth - 1;
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
