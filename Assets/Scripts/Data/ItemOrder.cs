using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemOrder {

    public string name { get; set; }
    public int amount, item;
    public City city;
    public ItemType type;
    public TradeDirection direction;
	public bool[] monthsActive;

    public ItemOrder(int n, int i, ItemType t) {

        amount = n;
        item = i;
        type = t;

        name = GetItemName();
		FreshMonthsActive();

    }

    public ItemOrder(int a, string i) {

        Node n = null;

        if (Enums.foodDict.ContainsKey(i))
            n = Enums.foodDict[i];

        else if (Enums.resourceDict.ContainsKey(i))
            n = Enums.resourceDict[i];

        else if (Enums.goodDict.ContainsKey(i))
            n = Enums.goodDict[i];

        else if (Enums.peopleDict.ContainsKey(i))
            n = Enums.peopleDict[i];

        else
            Debug.LogError(i + " does not exist as an item.");

        name = i;
        amount = a;
        item = (int)n.x;
        type = (ItemType)n.y;
		FreshMonthsActive();

	}

    public ItemOrder(string s) {

        if(String.IsNullOrEmpty(s))
            Debug.LogError("Item order input is null or empty.");
        string[] data = s.Split(' ');
        if (data.Length != 2)
            Debug.LogError("Bad data for item order.");
        int a = int.Parse(data[0]);
        string i = data[1];

        Node n = Enums.GetItemData(s);

        name = i;
        amount = a;
        item = (int)n.x;
        type = (ItemType)n.y;
		FreshMonthsActive();

	}

    public ItemOrder(int a, string s, City c, TradeDirection td) : this(a, s) {

        city = c;
        direction = td;

    }

	public void FreshMonthsActive() {

		monthsActive = new bool[12];
		for (int i = 0; i < 12; i++)
			monthsActive[i] = true;
		monthsActive[6] = false;

	}

    public string GetItemName() {

        return Enums.GetItemName(item, type);

	}

	public string GetItemDisplayName() {

		return Enums.GetItemDisplayName(item, type);

	}

	public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override bool Equals(object obj) {

        ItemOrder io = (ItemOrder)obj;
        return amount == io.amount && item == io.item && type == io.type && direction == io.direction && city == io.city;

    }

    public float ExchangeValue() {

        //get initial base value (item worth * amount)
        float baseValue = ResourcesDatabase.GetBasePrice(this);
        if (city != null)
            baseValue += city.distance;

        //if (city == null)
            return baseValue;

        //float modifier = city.attitude * 10;
        
        //if this is an import, we want to subtract the price if it's from a friendly city, not add to it
        //if (direction == TradeDirection.Import)
        //    modifier *= -1;
        //return baseValue * (100 + modifier) / 100;

        //if attitude is 2, modifier is 20
        //if an import, final price is 80% of base
        //otherwise it's 120%

    }

    public override string ToString() {

        return amount + " " + GetItemName() + " " + city + " " + direction + " " + type;

    }

}