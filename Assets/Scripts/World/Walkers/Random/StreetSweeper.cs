using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetSweeper : RandomWalker {

    public int cleanRate = 10;

    public override void VisitBuildings() {
        for (int a = X - radiusOfInfluence; a <= X + radiusOfInfluence; a++)
            for (int b = Y - radiusOfInfluence; b <= Y + radiusOfInfluence; b++)
                if (world.IsBuildingAt(a, b))
                    VisitBuilding(a, b);

    }

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        if (!world.IsUnblockedRoadAt(a, b))
            return;

        world.Map.cleanliness[a, b] -= (int)(cleanRate * ((Workplace)Origin).WorkerEffectiveness);

        if (world.Map.cleanliness[a, b] < 0)
            world.Map.cleanliness[a, b] = 0;

    }

}
