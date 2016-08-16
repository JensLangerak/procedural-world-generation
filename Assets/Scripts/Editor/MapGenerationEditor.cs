﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGenerationEditor : Editor {

	public override void OnInspectorGUI() {
		MapGenerator mapGen = (MapGenerator)target;

		if (DrawDefaultInspector ()) {
			if (mapGen.autoUpdate) {
				mapGen.generateMap ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			mapGen.generateMap ();
		}
	}
}
