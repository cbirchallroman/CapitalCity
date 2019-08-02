using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellWalker : RandomWalker {

    public Quality waterQuality;

    public float Multiplier { get { return 2 - (float)waterQuality * 0.5f * Difficulty.GetModifier(); } }

    public override void Activate() {
        base.Activate();
    }

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        House h = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (h == null)
            return;

        //give water
        if (h.WaterQual == waterQuality || h.waterQualWanted <= waterQuality)
            h.AddWater(h.WaterNeeded(waterQuality), waterQuality);

        UpdateCleanliness();

        //give disease
        string spot = a + "_" + b;
        if (VisitedSpots.Contains(spot))
            return;
        int roll = Random.Range(1, 100);
        if (yield == 0)
            roll = 100;
        if (roll <= yield)
            h.StartDisease();

        //house has been visited
        VisitedSpots.Add(spot);

    }

    void UpdateCleanliness() {

        string spot = X + "_" + Y;

        if (!world.Map.IsUnblockedRoadAt(X, Y) || VisitedSpots.Contains(spot))
            return;

        VisitedSpots.Add(spot);

        int localCleanliness = world.Map.cleanliness[X, Y];
        int previousAvg = yield;
        int tiles = VisitedSpots.Count;

        yield = (int)((tiles - 1) * previousAvg + localCleanliness * Multiplier) / tiles;
        //Debug.Log(spot + VisitedSpots.Contains(spot) + " - " + yield);

    }

}
