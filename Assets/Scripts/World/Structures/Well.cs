using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : Structure {

	public Quality waterQuality = Quality.Poor;
	public int waterQuantity = 1;

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        House house = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (house == null)
            return;

		if (house.WillAcceptWaterVisit(waterQuality))
			house.ReceiveWater(waterQuality, waterQuantity);

	}

}
