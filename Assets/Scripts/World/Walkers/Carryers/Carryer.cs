using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryer : Walker {

    public override void Activate() {
        base.Activate();

        if(Origin != null)
            Origin.ActiveSmartWalker = true;
    }

	public override void DoEveryStep() {

		if (Origin == null || Destination == null || !pathfinder.CanGoTo(X, Y, Prevx, Prevy, data))
			DestroySelf();

		if (Path.Count > 0)
			UpdatePathedMovement();

		else {

			//if not returning home yet (and therefore just reached destination), 
			if (!data.ReturningHome) {

				Stuck = true;

				//find path back home
				ReturnHome();

				//only procede if there's a way back home, otherwise don't continue
				if (Path.Count == 0)
					return;

				//perform action at destination
				OnceAtDestination();
				UpdatePathedMovement();

			}

			else if (data.ReturningHome)
				OnceBackHome();

		}

	}
	
    public override void DestroySelf() {
        base.DestroySelf();

        if (Origin != null)
            Origin.ActiveSmartWalker = false;

    }

    

}
