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


		if (market.typeStored == ItemType.Food)
			SellFood(house, market);
		else
			SellGoods(house, market);

		

	}

	void SellFood(House house, StorageBuilding market) {

		int hunger = house.Hunger;
		int numTypesHave = market.NumOfStoredTypes();

		//house will only proceed if we have more/equal food types or it is desperately hungry
		if (!house.WillAcceptFoodTypes(numTypesHave))
			return;

		int idealFoodPerType = hunger / numTypesHave;
		int[] inventory = market.Inventory;

		for(int item = 0; item < (int)FoodType.END && numTypesHave > 0; item++) {

			if (inventory[item] == 0)
				continue;

			numTypesHave--;

			int amount = inventory[item];

			//if the amount of this food is less than what we ideally want to sell per type
			if(amount < idealFoodPerType) {

				int diff = idealFoodPerType - amount;
				idealFoodPerType += diff / numTypesHave;	

			}



		}

	}

	void SellGoods(House house, StorageBuilding market) {

		for (int item = 0; item < (int)GoodType.END; item++) {

			//if the house does not want any of this item, don't sell
			SellItem(house, item, market.typeStored, market.Inventory[item]);

		}

	}


}
