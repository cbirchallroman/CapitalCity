using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceData {

	public string name;
	public int days = 16;
	public int weight = 1;
	public string[] ingredients;
	public Quality quality= Quality.Average;

	public ResourceData(Dictionary<string, string> contents, string[] ing) {

		name = contents["Name"];

		if(contents.ContainsKey("Days"))
			days = int.Parse(contents["Days"]);

		if (contents.ContainsKey("Weight"))
			weight = int.Parse(contents["Weight"]);
		//load ingredients
		ingredients = ing;

	}

}
