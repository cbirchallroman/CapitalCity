using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World {

    public Node size;
    public int[,] terrain, roads, cleanliness, desirability;
	public float[,] elevation;

    public World(float x, float y) : this(new Node((int)x, (int)y)) { }

    public World(Node sz) {

        size = sz;
        terrain = sz.CreateArrayOfSize<int>();
        roads = sz.CreateArrayOfSize<int>();
        cleanliness = sz.CreateArrayOfSize<int>();
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

    public Structure GetBuildingAt(int x, int y) {

		WorldController wc = GameObject.FindObjectOfType<WorldController>();
		return wc.GetBuildingAt(x, y);

	}

	public Structure GetBuildingAt(Node n) {

		return GetBuildingAt(n.x, n.y);

	}
	
}
