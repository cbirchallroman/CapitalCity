using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Machine {

	public string name;
	public MachineType type;
	public ResourceType material;
	public ResourceType fuel;
	public int fuelPer100;
	public int socialDays;
	public int deadLabor;

	public Machine(Dictionary<string, string> contents) {

		name = contents["Name"];

		type = Enums.GetMachineType(contents["Type"]);

		material = (ResourceType)Enums.GetItemData(contents["Material"]).x;
		
		socialDays = int.Parse(contents["Days"]);
		deadLabor = int.Parse(contents["DeadLabor"]);

		if (contents.ContainsKey("Fuel")) {

			fuel = (ResourceType)Enums.GetItemData(contents["Fuel"]).x;
			fuelPer100 = int.Parse(contents["FuelPer100"]);

		}

		else {

			fuel = ResourceType.END;
			fuelPer100 = 0;

		}

	}

	public int GetDeteriorationPerCycle(int stockpile) {
		
		//let's say we're contributing 6 out of 16 hours to producing 100 shoes (where 100 is generally how much of something we're making)
		//	and this machine has 180 hours of dead labor, from 100 bricks and 80 working hours
		//	each production cycle reduces the machine's value by 6 hours because that is consumed in production

		return (int)((float)stockpile * socialDays / 100);

	}

	public int GetTotalDeadLabor() {

		return ResourcesDatabase.GetAccumulativeDays(material.ToString()) + deadLabor;

	}

	public float ValueAddedToProduction(int stockpile) {

		int materialAmount = GetDeteriorationPerCycle(stockpile);
		int fuelAmount = (int)((float)stockpile * fuelPer100 / 100f);

		ItemOrder f = new ItemOrder(materialAmount, (int)material, ItemType.Resource);
		ItemOrder c = new ItemOrder(fuelAmount, (int)fuel, ItemType.Resource);

		Debug.Log(f.ExchangeValue() + " " + f);

		float value = f.ExchangeValue();
		if (fuel != ResourceType.END)   //only add value for fuel if it exists
			value += c.ExchangeValue();
		return value;

	}

}
