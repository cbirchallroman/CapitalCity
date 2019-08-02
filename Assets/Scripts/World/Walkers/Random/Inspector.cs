using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : RandomWalker {

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);
        Structure s = world.Map.GetBuildingAt(a, b).GetComponent<Structure>();
        s.FireRisk = 0;

    }
}
