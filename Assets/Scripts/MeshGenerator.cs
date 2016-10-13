using UnityEngine;
using System.Collections;

public class MeshGenerator {

	public static Mesh generateMesh(NoiseMapGenerator mapGenerator, int width, int height, Vector2 center, int detail, bool lowResTop, bool lowResRight, bool lowResBottom, bool lowResLeft)
	{
		//TODO method is too big (understatement), but is seems to work now
		//TODO normal calculation at the edges with lowRes is not correct.

		bool indent = false;

		//convert the detail to the number of skipped points
		detail = (detail > 7) ? 7 : (detail < 1) ? 1 : detail;
		detail = detail - 1;

		int detailSkippedPoints = (int) Mathf.Pow(2, detail);

		float h_sqrt_3 = Mathf.Sqrt(3.0f) * 0.5f;

		float centerX = ((float)width) / 2.0f;
		float centerY = ((float)height) / 2.0f * h_sqrt_3;

		//calculate the number of points in this chunk render
		int indexWidth = (width - 1) / detailSkippedPoints + 1;
		int indexHeight = (height - 1) / detailSkippedPoints + 1;

		//calculate the number of point that are (not) needed for the lowRes
		int extra = (lowResLeft) ? -(indexHeight / 4) : 0;
		extra += (lowResRight) ? (indexHeight / 4) : 0;

		//create vars for the mesh
		Vector3[] vertices = new Vector3[indexWidth * indexHeight + extra];
		int[] triangles = new int[((indexWidth - 1) * (indexHeight - 1) + extra) * 2 * 3];
		Vector2[] uv = new Vector2[indexWidth * indexHeight + extra];
		Vector3[] vertexNormals = new Vector3[vertices.Length];

		int verticeIndex = 0;
		int triangleIndex = 0;
		int extraTriangle = 0;

		//start creating the mesh
		for (int y = 0; y < indexHeight; y++)
		{
			//mesh is based in triangles, so there is a zigzag/tooth/saw pattern.
			float offset = (indent) ? 0.5f : 0f;
			for (int x = 0; x < indexWidth; x++)
			{

				float h = 0.0f;
				
				//If this is the top or bottom row, check if this row should be in a lower res, if so half of the point should be on a line between its neighbours. (or can be skipped, but this easier)
				if ((lowResTop && y == indexHeight - 1 || lowResBottom && y == 0) && (x % 2 == 1))
				{
					float h1 = mapGenerator.getPoint(new Vector2((x + offset - 1) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					float h2 = mapGenerator.getPoint(new Vector2((x + offset + 1) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					h = (h1 + h2) * 0.5f;
				}
				// if lowResLeft, some of the values should be the average of the point above and below.
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
				// if lowResRight, some of the values should be the average of the point above and below.
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
				//else get the height of this point
				else
				{
					h = mapGenerator.getPoint(new Vector2((((float)x) + offset) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
				}

				//due to the zigzag pattern, some points should not be drawn when lowResLeft is true. ZigZag pattern is larger in a lower res (so higher leverOfDetail var).
				if (lowResLeft && y % 4 == 2 && x == 0)
				{
					continue;
				} else {
					//create the vector for this point
					vertices[verticeIndex] = new Vector3((((float)x) + offset) * detailSkippedPoints - centerX, h * 10, ((float)y) * h_sqrt_3 * detailSkippedPoints - centerY);

					//get the surrounding points, needed for normal calculations. 
					//The default normal calculation does not work around the edges of the chunk so therefore I have to implement it myself.
					//not the most efficient solution, but it will work
					h = mapGenerator.getPoint(new Vector2((((float)x) - 0.5f + offset) * detailSkippedPoints, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 topLeft = new Vector3((((float)x)-0.5f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) +0.5f + offset) * detailSkippedPoints, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 topRight = new Vector3((((float)x)+0.5f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x)  -1f+ offset) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 left = new Vector3((((float)x)-1f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) +1f + offset) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 right = new Vector3((((float)x)+1f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) - 0.5f + offset) * detailSkippedPoints, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 bottomLeft = new Vector3((((float)x)-0.5f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) + 0.5f + offset) * detailSkippedPoints, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 bottomRight = new Vector3((((float)x) + 0.5f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints - centerY);

					//calculate normals of the triangles
					Vector3 normal = calulateTriangleNormal(topLeft, topRight, vertices[verticeIndex]);
					normal = calulateTriangleNormal(left, topLeft, vertices[verticeIndex]);
					normal = calulateTriangleNormal(topRight, right, vertices[verticeIndex]);
					normal = calulateTriangleNormal(bottomLeft, left, vertices[verticeIndex]);
					normal = calulateTriangleNormal(bottomRight, bottomLeft, vertices[verticeIndex]);
					normal = calulateTriangleNormal(right, bottomRight, vertices[verticeIndex]);

					//calculate the vertece normal
					vertexNormals[verticeIndex] += calulateTriangleNormal(topLeft, topRight, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(left, topLeft, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(topRight, right, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(bottomLeft, left, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(bottomRight, bottomLeft, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(right, bottomRight, vertices[verticeIndex]);

					vertexNormals[verticeIndex].Normalize();
				}

				/* when low res left or right one point (and triangle) is removed/added, offset needed to get the correct points */
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

			//create triangles
			if (y > 0)
				{
					if (indent)
					{
						//add the triangles for an indented row
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
						//add the triangles for a non indented row
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

				//when low res right the zigzig pattern goes further to the right, add extra point and triangle
				if (lowResRight && y % 4 == 2 && x == indexWidth - 1)
				{
					x++;
					h = mapGenerator.getPoint(new Vector2((x + offset) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					vertices[verticeIndex] = new Vector3((((float)x) + offset) * detailSkippedPoints - centerX, h * 10, ((float)y) * h_sqrt_3 * detailSkippedPoints - centerY);
					triangleIndex = addTriangleLeft(triangleIndex, verticeIndex, indexWidth, indent, triangles, extraTriangle);
					uv[verticeIndex] = new Vector2(x * detailSkippedPoints / (float)width, y * detailSkippedPoints / (float)height);

					h = mapGenerator.getPoint(new Vector2((((float)x) - 0.5f + offset) * detailSkippedPoints, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 topLeft = new Vector3((((float)x) - 0.5f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) + 0.5f + offset) * detailSkippedPoints, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 topRight = new Vector3((((float)x) + 0.5f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y + 1) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) - 1f + offset) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 left = new Vector3((((float)x) - 1f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) + 1f + offset) * detailSkippedPoints, ((float)y) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 right = new Vector3((((float)x) + 1f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) - 0.5f + offset) * detailSkippedPoints, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 bottomLeft = new Vector3((((float)x) - 0.5f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints - centerY);

					h = mapGenerator.getPoint(new Vector2((((float)x) + 0.5f + offset) * detailSkippedPoints, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints), center);
					Vector3 bottomRight = new Vector3((((float)x) + 0.5f + offset) * detailSkippedPoints - centerX, h * 10, ((float)y - 1) * h_sqrt_3 * detailSkippedPoints - centerY);

					Vector3 normal = calulateTriangleNormal(topLeft, topRight, vertices[verticeIndex]);
					normal = calulateTriangleNormal(left, topLeft, vertices[verticeIndex]);
					normal = calulateTriangleNormal(topRight, right, vertices[verticeIndex]);
					normal = calulateTriangleNormal(bottomLeft, left, vertices[verticeIndex]);
					normal = calulateTriangleNormal(bottomRight, bottomLeft, vertices[verticeIndex]);
					normal = calulateTriangleNormal(right, bottomRight, vertices[verticeIndex]);

					vertexNormals[verticeIndex] += calulateTriangleNormal(topLeft, topRight, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(left, topLeft, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(topRight, right, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(bottomLeft, left, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(bottomRight, bottomLeft, vertices[verticeIndex]);
					vertexNormals[verticeIndex] += calulateTriangleNormal(right, bottomRight, vertices[verticeIndex]);

					vertexNormals[verticeIndex].Normalize();

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
		//mesh.RecalculateNormals();
		mesh.normals = vertexNormals;
		return mesh;
	}

	/**
	 * \----p\--/\
	 *  \ A/  \ /  \
	 *   \/----\/----\
	 *   
	 * If current point is p, add triangle A
	 */
	protected static int addTriangleLeft(int triangleIndex, int verticeIndex, int indexWidth, bool indent, int[] triangles, int extraTriangle = 0)
	{
		int indentOffset = (indent) ? 0 : 1;
		indentOffset += extraTriangle;
		triangles[triangleIndex] = verticeIndex - indexWidth - indentOffset;
		triangles[triangleIndex + 1] = verticeIndex - 1;
		triangles[triangleIndex + 2] = verticeIndex;

		return triangleIndex + 3;
	}

	/**
	 * \----p\--/\
	 *  \  / B\ /  \
	 *   \/----\/----\
	 *   
	 * If current point is p, add triangle B
	 */
	protected static int addTriangleDown(int triangleIndex, int verticeIndex, int indexWidth, bool indent, int[] triangles, int extraTriangle = 0)
	{
		int indentOffset = (indent) ? 0 : 1;
		indentOffset += extraTriangle;
		triangles[triangleIndex] = verticeIndex - indexWidth + 1 - indentOffset;
		triangles[triangleIndex + 1] = verticeIndex - indexWidth - indentOffset;
		triangles[triangleIndex + 2] = verticeIndex;

		return triangleIndex + 3;
	}

	

	protected static Vector3 calulateTriangleNormal(Vector3 pointA, Vector3 pointB, Vector3 pointC)
	{
		return Vector3.Cross(pointB - pointA, pointC - pointA).normalized;
	}
}
