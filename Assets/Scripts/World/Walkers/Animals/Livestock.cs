using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Livestock : Animal {

    public int dirtiness = 5;
    public int maxYield = 100;
    public bool eatsPlants = true;

    public override void Activate() {

        base.Activate();

        if (Origin is Stable) {

            Stable ls = (Stable)Origin;
            ls.Spawned++;

        }

        UpdateRandomMovement();
    }

	public override void DoEveryStep() {

		if (Origin == null || !pathfinder.CanGoTo(X, Y, Prevx, Prevy, data))
			DestroySelf();

		//random movement if walktime > 0
		if (lifeTime > 0) {
			lifeTime--;

			if (WalkTime > 0) {
				UpdateRandomMovement();
				WalkTime--;
			}
			else if (RestTime > 0) {
				Stuck = true;
				RestTime--;
				if (world.IsUnblockedRoadAt(X, Y))
					world.Map.cleanliness[X, Y] += 5;
			}
			else {
				Stuck = false;
				WalkTime = walkTimeMax;
				RestTime = restTimeMax;
			}

			if (yield < maxYield)
				EatPlant(X, Y);

		}

		//if time is up and there's no path, find one
		else if (Path == null)
			ReturnHome();

		//if a path is supposed to exist
		else if (data.ReturningHome) {

			//if the path is still there, follow it
			if (Path.Count > 0)
				UpdatePathedMovement();

			//otherwise kill it
			else
				DestroySelf();
		}

	}

	public override void DestroySelf() {

        base.DestroySelf();

        if(Origin is Stable) {

            Stable ls = (Stable)Origin;
            ls.AmountStored += yield;
            ls.Spawned--;

        }

    }

    public override void ReturnHome() {

        List<Node> entrances = Origin.GetAdjBareGroundTiles();
        if (entrances.Count == 0) {
            DestroySelf();
            return;
        }
        FindPathTo(entrances);
        data.ReturningHome = true;
        Stuck = true;

    }

    void EatPlant(int a, int b) {

        Structure s = world.GetBuildingAt(a, b);
        if (s == null)
            return;
        Crop c = s.GetComponent<Crop>();
        if (c == null)
            return;
        if (c.AmountToGrow + yield > maxYield)
            return;
        c.Harvest();
        yield += c.AmountToGrow;

    }

}
