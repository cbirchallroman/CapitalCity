using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyLot : Structure {

    public override void DoEveryDay() {

        if (RoadAccess() && !ActiveSmartWalker && !immigration.Contains(this))
            RequestImmigrant();

    }

    void TurnIntoHouse(Prole immigrant) {

		//demolish this and build new house
		float rot = transform.position.y;
		world.Demolish(X, Y);
        Structure str = world.SpawnStructure(immigration.startingHouse, X, Y, rot);

		Debug.Log(str);

        House newHouse = str.GetComponent<House>();
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

	public override void ReceiveImmigrant(Prole p) {

		TurnIntoHouse(p);

	}

}
