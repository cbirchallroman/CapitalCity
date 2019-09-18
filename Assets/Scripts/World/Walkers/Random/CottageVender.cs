using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CottageVender : RandomWalker {

	public override void VisitBuilding(int a, int b) {

		base.VisitBuilding(a, b);

		if (Origin.AmountStored == 0)
			return;

		House house = world.Map.GetBuildingAt(a, b).GetComponent<House>();
		if (house == null)
			return;

		Cottage cottage = (Cottage)Origin;
		Node itemData = Enums.GetItemData(cottage.product);
		SellItem(house, itemData.x, (ItemType)itemData.y, (int)cottage.AmountStored);

	}

}
