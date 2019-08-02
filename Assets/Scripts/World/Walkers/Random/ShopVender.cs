using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopVender : RandomWalker {
	
    public override void Activate() {

        base.Activate();

    }

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        if (Origin.AmountStored == 0)
            return;

        House h = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (h == null)
            return;

		Distributer d = (Distributer)Origin;
		Node itemData = Enums.GetItemData(d.item);
		SellItem(h, itemData.x, (ItemType)itemData.y, (int)d.AmountStored);

    }

}
