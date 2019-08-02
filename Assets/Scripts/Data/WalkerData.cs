using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class WalkerData {

	public string name;
	public bool RoadWalker = false;
	public bool WaterWalker = false;
	public bool CanGoDiagonal = false;
	public bool RandomWalker = false;
	public bool ReturningHome = false;
	public string CanPassThrough = null;

	public WalkerData(Dictionary<string, string> contents) {

		name = contents["Name"];
		if (contents.ContainsKey("RandomWalker")) {
			RandomWalker = bool.Parse(contents["RandomWalker"]);
			RoadWalker = bool.Parse(contents["RandomWalker"]);
		}
		if (contents.ContainsKey("RoadWalker"))
			RoadWalker = bool.Parse(contents["RoadWalker"]);
		if (contents.ContainsKey("WaterWalker"))
			WaterWalker = bool.Parse(contents["WaterWalker"]);
		if (contents.ContainsKey("CanGoDiagonal"))
			CanGoDiagonal = bool.Parse(contents["CanGoDiagonal"]);
		if (contents.ContainsKey("CanPassThrough"))
			CanPassThrough = contents["CanPassThrough"];

	}

	public WalkerData(string n) {

		name = n;

	}

}