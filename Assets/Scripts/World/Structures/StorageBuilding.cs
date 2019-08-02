using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageBuildingSave : WorkplaceSave {
    
    public int[] inventory, queue;
    public float[] willAccept;
    public bool[] willGet;

    public StorageBuildingSave(GameObject go) : base(go) {

        StorageBuilding s = go.GetComponent<StorageBuilding>();
        
        inventory = s.Inventory;
        queue = s.Queue;
        willAccept = s.WillAccept;
        willGet = s.WillGet;

    }

}


public class StorageBuilding : Workplace {

    //implement WillGet system for individual buildings

    //SERIALIZE ALL THESE
    [Header("Storage Building")]
    public int maxTypes;
    public float defaultPercent = 1;
    public ItemType typeStored;
    public int[] Inventory { get; set; }
    public int[] Queue { get; set; }
    public float[] WillAccept { get; set; }
    public bool[] WillGet { get; set; }

    public List<StorageBlock> storageBlocks;

    public override void Load(ObjSave o) {
        base.Load(o);

        StorageBuildingSave s = (StorageBuildingSave)o;
        Inventory = s.inventory;
        Queue = s.queue;
        WillAccept = s.willAccept;
        WillGet = s.willGet;
        
    }

    public override void DoEveryDay() {

        base.DoEveryDay();

		if (Operational && !ActiveSmartWalker && !ActiveRandomWalker)
			GetOrRemove();

    }

    public void GetOrRemove() {

        //count through inventory so long as carryerwalker isn't active
        for (int a = 0; a < NumOfTotalTypes; a++) {

            //only keep going if WillGet[a] is true
            if (!WillGet[a])
                continue;

            //if empty space, find other storage with stuff
            if (EmptySpaceFor(a) > (stockpile * .90f * WillAccept[a])) {

                ItemOrder io = new ItemOrder(EmptySpaceFor(a), a, typeStored);

                //if building is found, send getter
                SpawnGetter(io);

            }

            //else find storage to accept surplus
            else if(Inventory[a] > stockpile * WillAccept[a]) {

                int amountToRemove = (int)(Inventory[a] - (stockpile * WillAccept[a]));

                ItemOrder io = new ItemOrder(amountToRemove, a, typeStored);

				//if building is found, remove stuff from inventory and send giver
				Carryer cart = SpawnGiver(io);
				if (!ActiveSmartWalker)
					return;
				StorageBuilding strg = (StorageBuilding)cart.Destination;
				RemoveItem(io);
				UpdateVisibleGoods();

            }
        }

    }

    public override void Activate() {

        base.Activate();

        Inventory = new int[NumOfTotalTypes];
        Queue = new int[NumOfTotalTypes];

        WillAccept = new float[NumOfTotalTypes];
        //set all types to accept all
        for (int a = 0; a < NumOfTotalTypes; a++)
            WillAccept[a] = defaultPercent;

        WillGet = new bool[NumOfTotalTypes];
        //FireRisk = 100;

    }

    public int NumOfTotalTypes {
        get {

            if (typeStored == ItemType.Food)
                return (int)FoodType.END;

            if (typeStored == ItemType.Good)
                return (int)GoodType.END;

            if (typeStored == ItemType.Resource)
                return (int)ResourceType.END;

            return 0;
        }
    }

    //amount of overall empty space, both for food currently present and food about to be stored
    public int EmptySpace { get { return stockpile - TotalAmountStored() - AmountQueued(); } }

    //empty space for a particular type, both currently and futurely stored
    public int EmptySpaceFor(int a) {

        //actual space left, which is the max it will accept times the total potential space minus the amount stored
        int space = (int)(WillAccept[a] * stockpile) - Inventory[a] - Queue[a];

        //if cannot accept type at all, return 0
        if (WillAccept[a] == 0)
            return 0;

        //if the total potential space is less than what could be stored individually, return the total potential space
        if (EmptySpace < space)
            return EmptySpace;

        //otherwise return specifically for this type
        return space;

    }

    //sums up total inventory
    public int TotalAmountStored() {
        int sum = 0;
        foreach (int i in Inventory)
            sum += i;
        return sum;
    }

    //sums up total queue
    public int AmountQueued() {
        int sum = 0;
        foreach (int i in Queue)
            sum += i;
        return sum;
    }

    //how much of a type will this building accept
    public bool CanAcceptAmount(int num, int ft) {

        //if it can't accept it at all, return false
        if (WillAccept[ft] == 0)
            return false;

        if (NumOfStoredTypes() == maxTypes && Inventory[ft] == 0)
            return false;

        if (!Operational)
            return false;

        //else return if currently stored, futurely stored, and the num to be added is less than the storagespace types what it will accept
        return Inventory[ft] + Queue[ft] + num <= stockpile * WillAccept[ft] && num <= EmptySpace;

    }

    public int NumOfStoredTypes() {

        int s = 0;

        for (int b = 0; b < Inventory.Length; b++)
            if (Inventory[b] > 0)
                s++;

        return s;

    }
    
    public override void ReceiveItem(ItemOrder io) {

        //if does not accept this type of item, reject
        if (io.type != typeStored)
            Debug.Log(name + " does not store " + io.GetItemName());

        //else remove from queue and add to inventory
        Queue[io.item] -= io.amount;
        Inventory[io.item] += io.amount;
        UpdateVisibleGoods();
    }

	public void ExpectItem(ItemOrder io) {

		Queue[io.item] += io.amount;
		//Debug.Log(name + " expects to receive " + io);

	}

	public override void RemoveItem(ItemOrder io) {

		Inventory[io.item] -= io.amount;

	}

    public override void OpenWindow() {
		
		OpenWindow(UIObjectDatabase.GetUIElement("StorageWindow"));

	}

    public void UpdateVisibleGoods() {

        int totalBlocks = storageBlocks.Count;
        int blockIndex = 0;

        //go through inventory
        for(int i = 0; i < Inventory.Length; i++) {

            //calculate percentage and how many blocks to take up
            int stored = Inventory[i];
            float percent = (float)stored / stockpile;
            int numOfBlocks = (int)(percent * totalBlocks);

            for(int j = 0; j < numOfBlocks; j++) {

                if (blockIndex >= totalBlocks)
                    Debug.LogError("Storage block index is greater than the number of storage blocks " + name + " has.");
                storageBlocks[blockIndex].SetState(i);
                blockIndex++;

            }
            

        }

    }

}
