using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : RandomWalker {

    public int healthToGive;

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        House h = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (h == null)
            return;

    }

}
