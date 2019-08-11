using UnityEngine;

public class Road : TiledStructure {

    public string evolution;
    public string devolution;
    public int desirabilityNeeded;
    public int desirabilityWanted;
	public bool connectToOutside;

    public override bool NeighborCondition(int a, int b) {

		//if out of bounds, only connect if you're supposed to connect to the outside
		if(world.Map.OutOfBounds(a, b)) {
			if (!connectToOutside)
				return false;
		}

		//else check if there might be a ramp here
		else if(world.Map.elevation[a, b] != world.Map.elevation[X, Y]) {

			string str = world.Map.GetBuildingNameAt(a, b);
			if (!string.IsNullOrEmpty(str))
				if (str.Contains("Ramp"))
					return true;    //only return true if there's a ramp at that spot

			//otherwise don't
			return false;

		}

		return world.Map.IsRoadAt(a, b);
	}

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
