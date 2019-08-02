using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caravan : Carryer {

    public float transportCost = 10;

	public override void DoEveryStep() {

		if (Destination == null && !data.ReturningHome)
			LeaveMap();

		if (!pathfinder.CanGoTo(X, Y, data))
			DestroySelf();

		if (Path.Count > 0)
			UpdatePathedMovement();

		else {

			//if not returning home yet (and therefore just reached destination), 
			if (!data.ReturningHome) {

				//perform action at destination
				OnceAtDestination();

				//find path back home
				LeaveMap();

				//only procede if there's a way back home, otherwise don't continue
				if (Path.Count == 0)
					DestroySelf();

				UpdatePathedMovement();

			}
			else
				DestroySelf();

		}

	}

}
