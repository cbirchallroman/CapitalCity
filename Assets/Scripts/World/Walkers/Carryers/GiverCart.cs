using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiverCart : Carryer {

	public override void Activate() {

		base.Activate();

		if (Destination is StorageBuilding) {
			StorageBuilding o = (StorageBuilding)Destination;
			o.ExpectItem(Order);
		}

	}

	public override void OnceAtDestination() {
		
        Destination.ReceiveItem(Order);

    }

}
