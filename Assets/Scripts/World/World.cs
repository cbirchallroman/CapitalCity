using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World {

    public float frequency = 3;
    public float water = 20;
    public float lush = 10;
    public float grass = 10;
    public float mud = 10;
    public float sand = 50;

    public Node size;
    public int[,] terrain, roads, cleanliness, desirability;
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

    public void RandomizeTerrain() {

        float offset = Random.Range(0, 1000);

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++) {

                float nx = x / size.x * frequency + offset;
                float ny = y / size.y * frequency + offset;

                float noise = Mathf.PerlinNoise(nx, ny);

                noise *= 100;
                
                if(noise < water)
                    terrain[x,y] = 0;
                else if(noise < lush + water)
                    terrain[x, y] = 1;
                else if (noise < grass + lush + water)
                    terrain[x, y] = 2;
                else if (noise < mud + grass + lush + water)
                    terrain[x, y] = 3;
                else
                    terrain[x, y] = 4;

            }
                

        terrain[0, 0] = (int)Terrain.Grass;
        terrain[(int)size.x - 1, (int)size.y - 1] = (int)Terrain.Grass;

    }

    public void PlainTerrain() {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                terrain[x, y] = (int)Terrain.Grass;
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

        return roads[x, y] == 2;

    }

	public bool IsRoadblockAt(int x, int y) {

		if (OutOfBounds(x, y))
			return false;

		return roads[x, y]  == 1;

	}

    public GameObject GetBuildingAt(int x, int y) {
        string n = GetBuildingNameAt(x, y);

        if (n == null)
            return null;

        return GameObject.Find(n);
    }

    public GameObject GetBuildingAt(Node n) {

        return GetBuildingAt(n.x, n.y);

    }

    public string GetBuildingNameAt(int x, int y){
        if(OutOfBounds(x,y))
            return "";

        return structures[x, y];
    }

    public string GetBuildingNameAt(Node n) {
        return GetBuildingNameAt((int)n.x, (int)n.y);
    }

    public bool IsBuildingAt(int x, int y) {
        return !string.IsNullOrEmpty(GetBuildingNameAt(x, y));
    }

    public int TileCost(int x, int y) {
        if (IsRoadAt(x, y))
            return 1;
        return 2;
    }

    public int TileCost(Node n) {
        return TileCost(n.x, n.y);
    }

    public bool IsNearWater(int x, int y, int sizex, int sizey) {

        for (int a = x - 2; a < x + sizex + 2; a++)
            for (int b = y - 2; b < y + sizey + 2; b++) {
                if (OutOfBounds(a, b))
                    continue;
                if (terrain[a, b] == (int)Terrain.Water)
                    return true;
            }
                
        return false;

    }

    public bool IsOnTile(int x, int y, int sizex, int sizey, Terrain t) {

        for (int a = x; a < x + sizex; a++)
            for (int b = y; b < y + sizey; b++) {
                if (OutOfBounds(a, b))
                    continue;
                if (terrain[a, b] == (int)t)
                    return true;
            }

        return false;

    }

    public bool IsNearStructure(int x, int y, int sizex, int sizey, string str) {

        for (int a = x - 2; a < x + sizex + 2; a++)
            for (int b = y - 2; b < y + sizey + 2; b++) {
                if (OutOfBounds(a, b))
                    continue;
                string s = structures[a, b];
                if (string.IsNullOrEmpty(s))
                    continue;
                if (s.Contains(str))
                    return true;
            }

        return false;

    }

    public bool IsNearCanal(int x, int y, int sizex, int sizey) {

        for (int a = x - 2; a < x + sizex + 2; a++)
            for (int b = y - 2; b < y + sizey + 2; b++) {
                if (OutOfBounds(a, b))
                    continue;
                GameObject go = GetBuildingAt(a, b);
				if (go == null)
					continue;
                Canal c = go.GetComponent<Canal>();
                if (c == null)
                    continue;
                if (c.WaterAccess)
                    return true;
            }

        return false;

    }

    public int Fertility(int x, int y) {

        int fert = (int)Terrain.END + 1 - terrain[x, y];

        if (IsNearCanal(x, y, 2, 2) && fert < 5)
            fert++;

        return fert;

    }

}
