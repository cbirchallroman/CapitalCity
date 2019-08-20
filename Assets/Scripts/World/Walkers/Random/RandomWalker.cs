using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalker : Walker {

    public bool modifyLifeTime = true;

    public override void Activate() {

        base.Activate();

        if (Origin != null)
            Origin.ActiveRandomWalker = true;

        if (Origin is Workplace && modifyLifeTime) {

            Workplace w = (Workplace)Origin;
            lifeTime = (int)((float)w.WorkersCount / w.workersMax * w.WorkingDay / 8f * lifeTime);

        }

		FindStartingDirection();
		UpdateRandomMovement();

	}

	void FindStartingDirection() {

		//check N, S, E, W tiles
		Node buildingDirection = new Node(0, 0);

		for (int dx = -1; dx <= 1; dx++) {
			for (int dy = -1; dy <= 1; dy++) {

				//don't do current tile
				if (dx == 0 && dy == 0)
					continue;

				//don't do diagonal tiles
				if (dx != 0 && dy != 0)
					continue;

				int a = X + dx;
				int b = Y + dy;

				string tile = world.Map.GetBuildingNameAt(a, b);
				if (!string.IsNullOrEmpty(tile))
					if (tile.Equals(Origin.name))
						buildingDirection = new Node(dx, dy);
			}
		}
		
		Direction = Node.GetOppositeDirection(buildingDirection);

	}

	public override void DoEveryStep() {

		if (Origin == null || !pathfinder.CanGoTo(X, Y, Prevx, Prevy, data))
			DestroySelf();

		VisitBuildings();

		//random movement if walktime > 0
		if (lifeTime > 0) {
			lifeTime--;
			UpdateRandomMovement();
		}

		//if time is up and there's no path, find one
		else if (!data.ReturningHome)
			ReturnHome();

		//if a path is supposed to exist
		if (data.ReturningHome) {

			//if the path is still there, follow it
			if (Path.Count > 0)
				UpdatePathedMovement();

			//otherwise kill it
			else
				DestroySelf();
		}

		//if origin is a workplace and is not operational, kill
		if (Origin is Workplace) {
			if (!((Workplace)Origin).Operational && !name.Contains("LaborSeeker"))		//if this is not a labor seeker and workplace is not operational
				DestroySelf();
			else if (!((Workplace)Origin).ActiveBuilding)								//else if the building is inactive regardless of laborseeker
				DestroySelf();
		}

	}

	public virtual void VisitBuildings() {
        for (int a = X - radiusOfInfluence; a <= X + radiusOfInfluence; a++)
            for (int b = Y - radiusOfInfluence; b <= Y + radiusOfInfluence; b++)
                if(world.Map.IsBuildingAt(a,b) && !world.Map.IsRoadAt(a,b))
                    VisitBuilding(a, b);

    }

    public override void VisitBuilding(int a, int b) {

        House h = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        Workplace w = (Workplace)Origin;

        if (laborSeeker && w != null && h != null) {

            if (w.WorkersCount == w.workersMax) {

                //if this is ONLY a laborseeker and it is not returning home yet, make it go home by setting its lifetime to 0
                if (!data.ReturningHome && name.Contains("LaborSeeker"))
                    lifeTime = 0;

                //then don't bother continuing
                return;

            }

            foreach(Prole p in h.Residents) {

				if (p == null)
					continue;
				if (p.GetLaborScore(w.laborType) < w.minimumAbility)	//if this prole's labor type is not preferred and we will not hire proles with unpreferred types, continue
					continue;
                if (p.SeekingWork)
                    w.AddWorker(p);

            }

        }
            

    }


    public override void DestroySelf() {

        base.DestroySelf();

        if(Origin != null)
            Origin.ActiveRandomWalker = false;

        if (laborSeeker && Origin is Workplace) {
            Workplace w = (Workplace)Origin;
            w.Access = LaborPoints;
            //labor.CalculateWorkers();
        }
            
            
    }
}
