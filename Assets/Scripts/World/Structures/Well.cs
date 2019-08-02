using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : Structure {

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        House h = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (h == null)
            return;

        if (h.WaterQual == Quality.Poor || h.waterQualWanted == Quality.Poor)
            h.AddWater(h.WaterNeeded(Quality.Poor), Quality.Poor);

    }

}
