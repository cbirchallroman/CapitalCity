using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory {

	public int slotCapacity = 8;
	public ItemType typeStored = ItemType.Resource;
	public string[] whitelist;
	public InventorySlot[] Slots { get; set; }

	public Inventory() {

		//instantiate inventory slots
		Slots = new InventorySlot[slotCapacity];
		for (int i = 0; i < slotCapacity; i++)
			Slots[i] = new InventorySlot(this);

	}

	//NOTE: NEED TO ACCOUNT FOR ITEMORDERS WHICH ARE MORE THAN 400 BY DIVIDING THE AMOUNT AMONG MULTIPLE SLOTS
	public bool CanAccept(ItemOrder io) {

		int amountToAcceptTotal = io.amount;

		//if there's a whitelist, check to make sure this item is allowed
		if(whitelist.Length > 0) {

			bool whitelisted = false;

			//look at each whitelisted item
			foreach (string s in whitelist)
				if (s.Equals(io.GetItemName()))
					whitelisted = true;

			if (!whitelisted)
				return false;

		}

		//make sure that we can accept this type of item
		if (io.type != typeStored)
			return false;

		for (int i = 0; i < slotCapacity && amountToAcceptTotal == 0; i++) {

			int emptySpace = Slots[i].EmptySpace;
			int amountToTry = amountToAcceptTotal > emptySpace ? emptySpace : amountToAcceptTotal;	//only try to fit as much as we can into this slot
																									//	if the amount we're trying is greater than the space we have, only test for the space we have
			ItemOrder forThisSlot = new ItemOrder(amountToTry, io.item, io.type);

			if (Slots[i].CanAccept(io))
				amountToAcceptTotal -= amountToTry;		//if we can accept this amount, subtract from the total we're trying to get rid of

		}

		return amountToAcceptTotal == 0;

	}



}

[System.Serializable]
public class InventorySlot {
	
	public int capacity = 400;
	public int item;
	public int amount;
	public int queue;
	public ItemType Type { get; set; }

	public int TakenSpace { get { return amount + queue; } }
	public int EmptySpace { get { return capacity - TakenSpace; } }
	public bool IsBlankSlot { get { return item == -1 && amount == 0; } }

	public InventorySlot(Inventory inv) {

		Type = inv.typeStored;
		item = -1;
		amount = 0;

	}

	public bool CanAccept(ItemOrder io) {

		//if itemorder is the wrong category of item (food, goods, resources)
		if (io.type != Type) {
			Debug.LogError("Inventory slot tried to take in wrong type of item");
			return false;
		}

		//if itemorder is a different item than stored here (and we ARE storing something here)
		if (io.item != item && item != -1)
			return false;

		//if accepting this item would be more than we can hold
		if (io.amount > EmptySpace)
			return false;

		return true;

	}

	public void Enqueue(ItemOrder io) {

		if (!CanAccept(io))
			Debug.LogError("Enqueueing " + io + " without being able to accept");

		queue += io.amount;

	}

	public void Accept(ItemOrder io) {

		if (!CanAccept(io))
			Debug.LogError("Enqueueing " + io + " without being able to accept");

		queue -= io.amount;
		amount += io.amount;

	}

}