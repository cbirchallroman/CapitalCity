using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Machine {

	public string name;
	public MachineType type;
	public ResourceType material;
	public ResourceType fuel;
	public float improvement;
	public float efficiency = 2;
	public int fuelPer100;
	public int socialDays;

	public Machine(Dictionary<string, string> contents) {

		name = contents["Name"];

		type = Enums.GetMachineType(contents["Type"]);

		material = (ResourceType)Enums.GetItemData(contents["Material"]).x;

		improvement = float.Parse(contents["Improvement"]);
		efficiency = float.Parse(contents["Efficiency"]);
		socialDays = int.Parse(contents["Days"]);

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

		float materialDays = ResourcesDatabase.GetAccumulativeDays(material.ToString());
		return (int)((float)stockpile * socialDays / materialDays);	//somehow derive this from hours embedded in the machine material

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
