using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cemetary : Workplace {

	//stockpile here is the # of corpses this can hold
	//AmountStored is the # of corpses currently held
	//Queue is the # of corpses en route

	public bool CanAcceptCorpses(int num) {

		return Operational && (AmountStored + num + Queue) <= stockpile;

	}

	public void AcceptCorpses(int num) {

		Queue -= num;
		AmountStored += num;

	}

	public void QueueCorpses(int num) {

		Queue += num;

	}

}
