using System.Collections.Generic;
using UnityEngine;

public class Road : TiledStructure {

    public string evolution;
    public string devolution;
	public string ramp;
    public int desirabilityNeeded;
    public int desirabilityWanted;
	public bool connectToOutside;
	public bool turnIntoRamp = true;

    public override bool NeighborCondition(int a, int b) {

		//if out of bounds, only connect if you're supposed to connect to the outside
		if(world.Map.OutOfBounds(a, b)) {
			if (!connectToOutside)
				return false;
		}

		//else check if there might be a ramp here
		else if(world.Map.roads[a, b] > 0) {

			GameObject str = world.Map.GetBuildingAt(a, b);
			if (str != null)
				if (str.name.Contains("Ramp")) {

					//now we check depending on the direction of the ramp and what side of it we're on
					float rot = str.transform.eulerAngles.y;

					return RampNeighbor(X, Y, a, b, rot);

				}

			//if this very road is a ramp, switch around the arguments
			else if (name.Contains("Ramp")) {

					float rot = transform.eulerAngles.y;
					return RampNeighbor(a, b, X, Y, rot);

				}

		}

		return world.Map.IsRoadAt(a, b) && world.Map.elevation[X, Y] == world.Map.elevation[a, b];
	}

	public bool RampNeighbor(int roadx, int roady, int rampx, int rampy, float rot) {
		
		float localElev = world.Map.elevation[roadx, roady];    //elevation on this tile
		float otherElev = world.Map.elevation[rampx, rampy];    //elevation on the ramp tile
		float delta = localElev - otherElev;
		bool lower = delta == 0;
		//if 0, this tile is on the lower side of the ramp

		if (rot == 0) {     //1, 0
			if (lower) {
				return roadx < rampx && roady == rampy;
			}
			else {
				return roadx > rampx && roady == rampy;
			}
		}
		else if (rot == 90) {    //0, -1
			if (lower) {
				return roady > rampy && roadx == rampx;
			}
			else {
				return roady < rampy && roadx == rampx;
			}
		}
		else if (rot == 180) {   //-1, 0
			if (lower) {
				return roadx > rampx && roady == rampy;
			}
			else {
				return roadx < rampx && roady == rampy;
			}
		}
		else if (rot == 270) {   //0, 1
			if (lower) {
				return roady < rampy && roadx == rampx;
			}
			else {
				return roady > rampy && roadx == rampx;
			}
		}

		return false;   //otherwise return false, if we somehow get to this point despite all the above

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

		if (turnIntoRamp)
			CheckRamp();

    }

    Structure Change(string s) {
        world.PlaceOnRoads = true;
        world.Destroy(X, Y);
        return world.SpawnStructure(s, X, Y, 0);
        //Debug.Log(name + " changed into " + s);
    }

	void CheckRamp() {

		List<Node> checks = new List<Node>();

		checks.Add(new Node(-1, 0));
		checks.Add(new Node(+1, 0));
		checks.Add(new Node(0, -1));
		checks.Add(new Node(0, +1));

		//save local elevation
		float localElev = world.Map.elevation[X, Y];

		//check each possible case, then check for a tile on the opposite end that's higher/shorter than the tile we're on
		foreach (Node c in checks) {
			
			int a = X + c.x;
			int b = Y + c.y;

			//if out of bounds, don't check
			if (world.Map.OutOfBounds(a, b)) continue;

			//if no road here, don't check
			if (world.Map.roads[a, b] == 0) continue;

			float hereElev = world.Map.elevation[a, b];

			//if the elevation here is 1 higher than at our tile, build a ramp instead
			if (hereElev != localElev + 1)
				continue;

			//now find the ramp's rotation
			float rot = 0;
			if (c.x == -1 && c.y == 0)
				rot = 180;
			else if (c.x == 1 && c.y == 0)
				rot = 0;
			else if (c.x == 0 && c.y == 1)
				rot = 270;
			else if (c.x == 0 && c.y == -1)
				rot = 90;
			
			bool allow = false;
			if (Neighbors == 0)
				allow = true;
			else if (Neighbors == 1 && rot == 0)
				allow = true;
			else if (Neighbors == 2 && rot == 90)
				allow = true;
			else if (Neighbors == 4 && rot == 180)
				allow = true;
			else if (Neighbors == 8 && rot == 270)
				allow = true;

			if (!allow)
				continue;

			Structure rmp = Change(ramp);
			rmp.transform.rotation = Quaternion.Euler(new Vector3(0, rot, 0));

		}

	}
    
}
