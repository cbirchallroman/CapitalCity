using UnityEngine;

public class Road : TiledStructure {

    public string evolution;
    public string devolution;
    public int desirabilityNeeded;
    public int desirabilityWanted;
	public bool connectToOutside;

    public override bool NeighborCondition(int a, int b) { return world.Map.IsRoadAt(a, b) || (world.Map.OutOfBounds(a, b) && connectToOutside); }

    public override void Activate() {

        base.Activate();

        world.Map.roads[X, Y] = 2;

    }

    public new void Update() {

        UpdateTiling();

        if (world.Map.roads[X, Y] != 2)
            world.Map.roads[X, Y] = 2;

        if (world.Map.desirability[X, Y] >= desirabilityWanted && !string.IsNullOrEmpty(evolution))
            Change(evolution);

        if (world.Map.desirability[X, Y] < desirabilityNeeded && !string.IsNullOrEmpty(devolution))
            Change(devolution);

    }

    void Change(string s) {
        world.PlaceOnRoads = true;
        world.Destroy(X, Y);
        world.SpawnStructure(s, X, Y, 0);
        //Debug.Log(name + " changed into " + s);
    }
    
}
