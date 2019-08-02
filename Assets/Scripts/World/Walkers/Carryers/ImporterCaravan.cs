using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImporterCaravan : Caravan {

	public override void Activate() {

		base.Activate();

		if (Destination is StorageBuilding) {
			StorageBuilding o = (StorageBuilding)Destination;
			o.ExpectItem(Order);
		}

	}

	public override void OnceAtDestination() {

        base.OnceAtDestination();
        Destination.ReceiveItem(Order);
        float totalCost = Order.ExchangeValue();
        money.SpendOnImports(totalCost, Order.type);

    }

}
