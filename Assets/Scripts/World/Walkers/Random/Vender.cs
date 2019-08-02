using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vender : RandomWalker {

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

		StorageBuilding market = (StorageBuilding)Origin;
		House house = world.Map.GetBuildingAt(a, b).GetComponent<House>();
		if (house == null)
			return;
		int numTypes = market.typeStored == ItemType.Food ? (int)FoodType.END : (int)GoodType.END;

		//go through inventory
		for (int item = 0; item < numTypes; item++) {

			//if the house does not want any of this item, don't sell
			SellItem(house, item, market.typeStored, market.Inventory[item]);
			
		}

	}


}
