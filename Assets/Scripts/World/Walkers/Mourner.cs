using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mourner : Carryer {

	public override void Activate() {

		base.Activate();

		if(Destination is Cemetary) {

			((Cemetary)Destination).QueueCorpses(yield);

		}

	}

	public override void OnceAtDestination() {
		
		if(Destination is Cemetary) {

			((Cemetary)Destination).AcceptCorpses(yield);

		}

	}

}
