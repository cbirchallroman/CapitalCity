using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distributer : Workplace {

	public string item;

	public override void DoEveryDay() {

		base.DoEveryDay();

		if (Operational && !ActiveSmartWalker && !ActiveRandomWalker)
			GetItem();

	}

	void GetItem() {

		if (AmountStored > .1f * stockpile)
			return;

		ItemOrder io = new ItemOrder(stockpile - (int)AmountStored, item);
		SpawnGetterToStorage(io);

	}

	public override void ReceiveItem(ItemOrder io) {

		if (io.GetItemName() != item)
			Debug.LogError(name + " received " + io + " in error");
		AmountStored += io.amount;

	}

	public override void RemoveItem(ItemOrder io) {

		if (io.GetItemName() != item)
			Debug.LogError(name + " removed " + io + " in error");
		AmountStored -= io.amount;

	}

}
