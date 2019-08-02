using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Architect : RandomWalker {

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);
        Structure s = world.Map.GetBuildingAt(a, b).GetComponent<Structure>();
        if (s.name.Contains("RubbleOnFire"))
            return;
        s.CollapseRisk = s.collapseRiskMax;

    }
}
