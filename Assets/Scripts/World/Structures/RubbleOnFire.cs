using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbleOnFire : Structure {

    public override void Activate() {

        base.Activate();

        CollapseRisk = collapseRiskMax;

    }

    public override void DoEveryMonth() {
        


    }

    public override void DoEveryDay() {

        CollapseRisk--;

        if (CollapseRisk <= 0)
            TurnToRubble();

    }

    public override void VisitBuilding(int a, int b) {

        Structure s = world.Map.GetBuildingAt(a, b).GetComponent<Structure>();
        if(!s.name.Contains("Rubble") && !s.name.Contains("Road"))
            world.Map.GetBuildingAt(a, b).GetComponent<Structure>().FireRisk += 1.25f;

    }
}
