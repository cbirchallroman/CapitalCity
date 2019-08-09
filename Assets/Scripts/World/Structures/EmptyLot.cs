using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyLot : Structure {

    public override void DoEveryDay() {

        if (RoadAccess() && !ActiveSmartWalker && !immigration.Contains(this))
            RequestImmigrant();

    }

    void TurnIntoHouse(Adult immigrant) {

		//demolish this and build new house
		float rot = transform.position.y;
		world.Demolish(X, Y);
        world.SpawnStructure(immigration.startingHouse, X, Y, rot);

        House newHouse = world.Map.GetBuildingAt(X, Y).GetComponent<House>();
        newHouse.FreshHouse(immigrant);

    }

    bool RoadAccess() {
        for (int a = X - 2; a <= X + 2; a++)
            for (int b = Y - 2; b <= Y + 2; b++)
                if (world.Map.IsRoadAt(a, b))
                    if(!world.Map.structures[a,b].Contains("MapE"))
                        return true;
        return false;
    }

	public override void ReceiveImmigrant(Adult p) {

		TurnIntoHouse(p);

	}

}
