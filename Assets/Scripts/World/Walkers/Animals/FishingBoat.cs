using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingBoat : Livestock {
	
	public override void DoEveryStep() {

		if (Origin == null || !pathfinder.CanGoTo(X, Y, Prevx, Prevy, data))
			DestroySelf();

		string spot = X + "_" + Y;
		if (!VisitedSpots.Contains(spot))
			VisitedSpots.Add(spot);

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
			}
			else {
				Stuck = false;
				WalkTime = walkTimeMax;
				RestTime = restTimeMax;
			}

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

	public override void ReturnHome() {

        List<Node> entrances = Origin.GetAdjWaterTiles();
        if (entrances.Count == 0) {
            DestroySelf();
            return;
        }
        FindPathTo(entrances);
        data.ReturningHome = true;
        Stuck = true;

    }

    public override void DestroySelf() {

        base.DestroySelf();

        if (Origin is Stable) {

            Stable ls = (Stable)Origin;
            ls.AmountStored += VisitedSpots.Count * 5;
            ls.Spawned--;

        }

    }

}
