using UnityEngine;
using System.Collections;

public class MeshGenerator {

	public static Mesh generateMesh(NoiseMapGenerator mapGenerator, int width, int height, Vector2 center, int detail, bool lowResTop, bool lowResRight, bool lowResBottom, bool lowResLeft)
	{
		bool indent = false;
		detail = (detail > 7) ? 7 : (detail < 1) ? 1 : detail;
		detail = detail - 1;

		int detailSkippedPoints = (int) Mathf.Pow(2, detail);

		float h_sqrt_3 = Mathf.Sqrt(3.0f) * 0.5f;

		float centerX = ((float)width) / 2.0f;
		float centerY = ((float)height) / 2.0f * h_sqrt_3;

		int indexWidth = (width - 1) / detailSkippedPoints + 1;
		int indexHeight = (height - 1) / detailSkippedPoints + 1;
		int extra = (lowResLeft) ? -(indexHeight / 4) : 0;
		extra += (lowResRight) ? (indexHeight / 4) : 0;
		Vector3[] vertices = new Vector3[indexWidth * indexHeight + extra];
		int[] triangles = new int[((indexWidth - 1) * (indexHeight - 1) + extra) * 2 * 3];
		Vector2[] uv = new Vector2[indexWidth * indexHeight + extra];
		int verticeIndex = 0;
		int triangleIndex = 0;
		int extraTriangle = 0;

		for (int y = 0; y < indexHeight; y++)
		{
			float offset = (indent) ? 0.5f : 0f;
			for (int x = 0; x < indexWidth; x++)
			{

				float h = 0.0f;
				if ((lowResTop && y == 0 || lowResBottom && y == indexHeight - 1) && (x % 2 == 1))
				{
					float h1 = mapGenerator.getPoint(new Vector2((x + offset - 1) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					float h2 = mapGenerator.getPoint(new Vector2((x + offset + 1) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					h = (h1 + h2) * 0.5f;
				}
				else if (lowResLeft && x == 0 && y % 4 == 1)
				{

					float h1 = mapGenerator.getPoint(new Vector2(x * detailSkippedPoints, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints), center);
					float h2 = mapGenerator.getPoint(new Vector2((x + 1) * detailSkippedPoints, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints), center);
					h = (h1 + h2) * 0.5f;
				}
				else if (lowResLeft && x == 0 && y % 4 == 3)
				{
					float h1 = mapGenerator.getPoint(new Vector2((x + 1) * detailSkippedPoints, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints), center);
					float h2 = mapGenerator.getPoint(new Vector2(x * detailSkippedPoints, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints), center);
					h = (h1 + h2) * 0.5f;
				}
				else if (lowResRight && y % 4 == 1 && x == indexWidth - 1)
				{
					float h1 = mapGenerator.getPoint(new Vector2(x * detailSkippedPoints, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints), center);
					float h2 = mapGenerator.getPoint(new Vector2((x + 1) * detailSkippedPoints, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints), center);
					h = (h1 + h2) * 0.5f;
				}
				else if (lowResRight && y % 4 == 3 && x == indexWidth - 1)
				{
					float h1 = mapGenerator.getPoint(new Vector2((x + 1) * detailSkippedPoints, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints), center);
					float h2 = mapGenerator.getPoint(new Vector2(x * detailSkippedPoints, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints), center);
					h = (h1 + h2) * 0.5f;
				}
				else
				{
					h = mapGenerator.getPoint(new Vector2((x + offset) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
				}

				if (lowResLeft && y % 4 == 2 && x == 0)
				{
					continue;
				} else {
					vertices[verticeIndex] = new Vector3((((float)x) + offset) * detailSkippedPoints - centerX, h * 10, ((float)y) * h_sqrt_3 * detailSkippedPoints - centerY);
				}

				/* when low res left or right one point (and triangle) is removed/added */
				if (lowResLeft && y % 4 == 2 )
				{
					extraTriangle = -1;
				} else if (lowResRight && y % 4 == 3)
				{
					extraTriangle = 1;
				} else
				{
					extraTriangle = 0;
				}

			if (y > 0)
				{
					if (indent)
					{
					
						if (x >= 1)
						{
							triangleIndex = addTriangleLeft(triangleIndex, verticeIndex, indexWidth, indent, triangles, extraTriangle);
						}
						if (x < indexWidth - 1)
						{
							if (!(x == 0 && lowResLeft && y % 4 == 3))
							{
								triangleIndex = addTriangleDown(triangleIndex, verticeIndex, indexWidth, indent, triangles, extraTriangle);
							}
						}

					}
					else
					{
						
						if (x >= 1)
						{

							if (!(x == 1 && lowResLeft && y % 4 == 2))
							{
								triangleIndex = addTriangleLeft(triangleIndex, verticeIndex, indexWidth, indent, triangles, extraTriangle);
							}

							triangleIndex = addTriangleDown(triangleIndex, verticeIndex, indexWidth, indent, triangles, extraTriangle);

						}
					
					}

				}
				uv[verticeIndex] = new Vector2(x * detailSkippedPoints / (float)width, y * detailSkippedPoints / (float)height);

				verticeIndex++;

				if (lowResRight && y % 4 == 2 && x == indexWidth - 1)
				{
					x++;
					h = mapGenerator.getPoint(new Vector2((x + offset) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					vertices[verticeIndex] = new Vector3((((float)x) + offset) * detailSkippedPoints - centerX, h * 10, ((float)y) * h_sqrt_3 * detailSkippedPoints - centerY);
					triangleIndex = addTriangleLeft(triangleIndex, verticeIndex, indexWidth, indent, triangles, extraTriangle);
					uv[verticeIndex] = new Vector2(x * detailSkippedPoints / (float)width, y * detailSkippedPoints / (float)height);

					verticeIndex++;
				} else if (lowResRight && y % 4 == 3 && x == indexWidth - 1)
				{
					triangleIndex = addTriangleDown(triangleIndex, verticeIndex - 1, indexWidth, indent, triangles, extraTriangle);

				}
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

	protected static int addTriangleLeft(int triangleIndex, int verticeIndex, int indexWidth, bool indent, int[] triangles, int extraTriangle = 0)
	{
		int indentOffset = (indent) ? 0 : 1;
		indentOffset += extraTriangle;
		triangles[triangleIndex] = verticeIndex - indexWidth - indentOffset;
		triangles[triangleIndex + 1] = verticeIndex - 1;
		triangles[triangleIndex + 2] = verticeIndex;

		return triangleIndex + 3;
	}
	protected static int addTriangleDown(int triangleIndex, int verticeIndex, int indexWidth, bool indent, int[] triangles, int extraTriangle = 0)
	{
		int indentOffset = (indent) ? 0 : 1;
		indentOffset += extraTriangle;
		triangles[triangleIndex] = verticeIndex - indexWidth + 1 - indentOffset;
		triangles[triangleIndex + 1] = verticeIndex - indexWidth - indentOffset;
		triangles[triangleIndex + 2] = verticeIndex;

		return triangleIndex + 3;
	}
}
