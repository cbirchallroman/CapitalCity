using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class StructureData {

	public int sizex;
	public int sizey;
	public int baseCost;

    public float Alignx { get { return .5f * (sizex - 1); } }
    public float Aligny { get { return .5f * (sizey - 1); } }

	public bool canDrag;
	public bool straightLineDrag;
	public bool hasWaterTiles;
	public bool hasRoadTiles;
	public bool builtOnce;
	public bool buildNearWater;

	public string name;
	public string displayName;
	public string placeOnTerrain;
	public string placeNearby;
	public int[,] waterTiles;
	public int[,] roadTiles;

    public StructureData(Dictionary<string, string> contents) {

        name = contents["Name"];
        displayName = contents["DisplayName"];

        sizex = int.Parse(contents["Sizex"]);
        sizey = int.Parse(contents["Sizey"]);
        baseCost = int.Parse(contents["BaseCost"]);

        if (contents.ContainsKey("CanDrag"))
            canDrag = bool.Parse(contents["CanDrag"]);
		if (contents.ContainsKey("StraightLineDrag"))
			straightLineDrag = bool.Parse(contents["StraightLineDrag"]);
		if (contents.ContainsKey("BuiltOnce"))
            builtOnce = bool.Parse(contents["BuiltOnce"]);
        if (contents.ContainsKey("BuildNearWater"))
            buildNearWater = bool.Parse(contents["BuildNearWater"]);
        if (contents.ContainsKey("PlaceNearby"))
            placeNearby = contents["PlaceNearby"];

        if (contents.ContainsKey("HasWaterTiles"))
            hasWaterTiles = bool.Parse(contents["HasWaterTiles"]);
        if (hasWaterTiles)
            LoadWaterTiles();

        if (contents.ContainsKey("HasRoadTiles"))
            hasRoadTiles = bool.Parse(contents["HasRoadTiles"]);
        if (hasRoadTiles)
            LoadRoadTiles();

        if (contents.ContainsKey("PlaceOnTerrain"))
            placeOnTerrain = contents["PlaceOnTerrain"];

    }

    public void LoadWaterTiles() {
        
        TextAsset file = Resources.Load<TextAsset>("BuildingData/Water/" + name);
        waterTiles = new int[sizex, sizey];

        int a = 0;
        int b = 0;

        foreach(string s in new List<string>(file.text.Split(' '))) {

            waterTiles[a, b] = int.Parse(s);

            a++;
            if(a >= sizex) {
                a = 0;
                b++;
            }
            if(b >= sizey)
                break;

        }

    }

    public void LoadRoadTiles() {

        TextAsset file = Resources.Load<TextAsset>("BuildingData/Roads/" + name);
        roadTiles = new int[sizex, sizey];

        int a = 0;
        int b = 0;

        foreach (string s in new List<string>(file.text.Split(' '))) {

            roadTiles[a, b] = int.Parse(s);

            a++;
            if (a >= sizex) {
                a = 0;
                b++;
            }
            if (b >= sizey)
                break;

        }


    }
}
