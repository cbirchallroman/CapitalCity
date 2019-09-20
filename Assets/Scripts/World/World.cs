using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World {

    public Node size;
    public int[,] terrain, roads, cleanliness, desirability;
	public float[,] elevation;
    public string[,] structures;

    public World(float x, float y) : this(new Node((int)x, (int)y)) { }

    public World(Node sz) {

        size = sz;
        terrain = sz.CreateArrayOfSize<int>();
        roads = sz.CreateArrayOfSize<int>();
        cleanliness = sz.CreateArrayOfSize<int>();
        structures = sz.CreateArrayOfSize<string>();
        desirability = sz.CreateArrayOfSize<int>();

    }

    public bool OutOfBounds(int x, int y) {
        return x < 0 || x >= size.x || y < 0 || y >= size.y;
    }

    public bool OutOfBounds(int x, int y, int szx, int szy) {

        for (int x2 = x; x2 < szx + x; x2++) {
            for (int y2 = y; y2 < szy + y; y2++) {
                if (OutOfBounds(x2, y2))
                    return true;
            }

        }

        return false;
    }

    public void RenameArea(string s, int x, int y, int szx, int szy) {
        structures[x, y] = s;
        for (int a = x; a < szx + x; a++) {
            for (int b = y; b < szy + y; b++) {
                structures[a, b] = s;
            }
        }
    }

    void ClearArea(int x, int y, int szx, int szy) {
        RenameArea(null, x, y, szx, szy);
    }

    public bool IsRoadAt(int x, int y) {

        if (OutOfBounds(x, y))
            return false;

        return roads[x, y] > 0;

    }

    public bool IsUnblockedRoadAt(int x, int y) {

        if (OutOfBounds(x, y))
            return false;

		if (IsBuildingAt(x, y))
			if (GetBuildingNameAt(x, y).Contains("Ramp"))
				return false;

        return roads[x, y] == 2;

    }

	public bool IsRoadblockAt(int x, int y) {

		if (OutOfBounds(x, y))
			return false;

		return roads[x, y]  == 1;

	}

    public string GetBuildingNameAt(int x, int y){
        if(OutOfBounds(x,y))
            return "";

        return structures[x, y];
    }

    public string GetBuildingNameAt(Node n) {
        return GetBuildingNameAt(n.x, n.y);
    }

    public int TileCost(int x, int y) {
        if (IsRoadAt(x, y))
            return 1;
        return 2;
    }

    public int TileCost(Node n) {
        return TileCost(n.x, n.y);
    }
	
}
