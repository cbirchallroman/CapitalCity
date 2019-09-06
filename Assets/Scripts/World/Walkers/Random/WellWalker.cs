using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellWalker : RandomWalker {

    public Quality waterQuality;
	public int waterQuantity = 10;

    public float Multiplier { get { return 2 - (float)waterQuality * 0.5f * Difficulty.GetModifier(); } }

    public override void Activate() {
        base.Activate();
    }

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

		//check if spot visited
		string spot = a + "_" + b;
		if (VisitedSpots.Contains(spot))
			return;
		VisitedSpots.Add(spot);		//house has been visited

		House house = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (house == null)
            return;

        //give water
        if (house.WillAcceptWaterVisit(waterQuality))
            house.ReceiveWater(waterQuality, waterQuantity);
        UpdateCleanliness();    //yield is the average filthiness of roads that the wellwalker has walked on

		//give disease
		if (yield != 0)
			TryDisease(house);


    }

	void TryDisease(House house) {
		
		foreach(Prole res in house.Residents) {
			
			TryDiseaseAtPerson(res, house);
			foreach (Child child in res.children)
				TryDiseaseAtPerson(child, house);

		}

	}

	void TryDiseaseAtPerson(Person p, House house) {

		if (p.diseased)
			return;

		if (Random.Range(1, 100) + house.Hygiene <= yield) {

			p.TurnDiseased();
			house.AddDiseasedResident();

		}	//want to roll low to cause disease

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
