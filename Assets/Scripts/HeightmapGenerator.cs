using UnityEngine;
using System.Collections;

public interface IHeightmapGenerator {
	float getPoint(Vector2 point, Vector2 offset);
}
