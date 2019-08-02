using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : Walker {

    public string targetType;
    public bool Acting { get; set; }

    public override void Activate() {
        base.Activate();

        if (Origin != null)
            Origin.ActiveSmartWalker = true;
    }

	public override void DoEveryStep() {

		if (Origin == null || !pathfinder.CanGoTo(X, Y, data))
			DestroySelf();

		if (Destination == null && !data.ReturningHome)
			GoToNext();

		if (Path.Count > 0)
			UpdatePathedMovement();

		else {

			//if not returning home yet (and therefore just reached destination), 
			if (!data.ReturningHome) {

				Stuck = true;

				//perform action at destination
				OnceAtDestination();
				GoToNext();

				//only procede if there's a way back home, otherwise don't continue
				if (Path.Count > 0)
					UpdatePathedMovement();

			}

			else if (data.ReturningHome)
				OnceBackHome();

		}

	}

	void GoToNext() {

		SimplePriorityQueue<Structure, float> queue = FindClosestStructureOfType(targetType);

		if (queue.Count == 0)
            ReturnHome();

        else {

			Destination = queue.Dequeue();
			
			Node end = new Node(Destination);

            FindPathTo(end);

            //if (Path.Count > 15)
            //    ReturnHome();

            Stuck = true;

        }

    }

    public override void DestroySelf() {
        base.DestroySelf();

        if (Origin != null)
            Origin.ActiveSmartWalker = false;

    }

}
