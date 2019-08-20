using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emigrant : Walker {

	public override void Activate() {

		base.Activate();

		Debug.Log(PersonData + " leaving city");

	}

	public override void DoEveryStep() {

		if (!pathfinder.CanGoTo(X, Y, Prevx, Prevy, data))
			DestroySelf();

		if (Path.Count > 0)
			UpdatePathedMovement();
		else
			DestroySelf();

	}

}
