using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immigrant : Walker {

    public override void Activate() {

        base.Activate();

        if (Destination != null)
            Destination.ActiveSmartWalker = true;

        UpdatePathedMovement();

		if (PersonData == null)
			Debug.LogError(name + " activated without being assigned a prole");
		//else
			Debug.Log(PersonData + " (Labor Pref: " + PersonData.laborPref + ") moving into " + Destination);

    }

	public override void DoEveryStep() {

		if (!pathfinder.CanGoTo(X, Y, data))
			FindPathTo(new Node(Destination));

		//if there's no house to go to, go to map exit
		if (Destination == null && !data.ReturningHome)
			DestroySelf();

		if (Path.Count > 0)
			UpdatePathedMovement();

		else {

			//if found destination, give immigrant and kill self
			if (X == Destination.X && Y == Destination.Y) {
				Destination.ReceiveImmigrant(PersonData);
				DestroySelf();
			}

			//else if you're at the end of the path but there's no house, go to map exit
			else if (!data.ReturningHome)
				LeaveMap();

			//once there, kill self
			else
				DestroySelf();

		}

	}

	void SendNewImmigrant() {

		if (Destination != null)
			Destination.RequestImmigrant();
		DestroySelf();

	}
    
}
