using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductivityController : MonoBehaviour {

	public static Dictionary<string, List<Structure>> generators;
	public static Dictionary<string, float> productivities;
	public static Dictionary<string, float> automationValue;
	public static Dictionary<string, List<City>> competitors;

    //create lists of structures which contribute to an item's productivity
    //  this should be done whenever the world loads; structures are added to the list upon creation and save-loading
    //  this isn't saved between games because (1) structures cannot be serialized and (2) we don't need to know this all of the time, only at the start of each quarter
	public static void LoadProductivityLists() {

		generators = new Dictionary<string, List<Structure>>();
		List<string> items = Enums.GetAllItems();
		foreach (string item in items)
			generators[item] = new List<Structure>();

	}

    //creates dictionary of items' productivities
    //  this is saved between games because we need this info all the time
    public static void CreateProductivities() {

        productivities = new Dictionary<string, float>();
		automationValue = new Dictionary<string, float>();

    }

    //load the saved productivities from other 
	public static void LoadProductivities(DictContainer<string, float> p, DictContainer<string, float> a) {
        
        productivities = p.GetDictionary();
		automationValue = a.GetDictionary();

    }

	public static void LoadCompetitors(List<City> cities) {

		competitors = new Dictionary<string, List<City>>();

		foreach (City c in cities) {

			List<string> products = c.imports;

			foreach(string product in products) {

				if (!competitors.ContainsKey(product))
					competitors[product] = new List<City>();
				competitors[product].Add(c);

			}

        }
        //UpdateProductivities();

    }

	public static float GetAverageProductivityHere(string item) {

		List<Structure> structs = generators[item];
		float currentAverageProd = 0;
		float currentIndex = 0;

		foreach(Structure str in structs) {

			float p = str.GetActualProductivity(item);

			//if no production, continue
			if (p == 0)
				continue;

			currentIndex++;
			currentAverageProd = (currentIndex - 1) * currentAverageProd + str.GetActualProductivity(item);
			currentAverageProd /= currentIndex;

		}

		return currentAverageProd;

	}

	public static float GetAverageValueAddedHere(string item) {

		List<Structure> structs = generators[item];
		float currentAverageValueAdded = 0;
		float currentIndex = 0;

		foreach (Structure str in structs) {

			float p = str.GetActualProductivity(item);

			//if no production, continue
			if (p == 0)
				continue;

			currentIndex++;
			currentAverageValueAdded = (currentIndex - 1) * currentAverageValueAdded + str.GetAutomationValue(item);
			currentAverageValueAdded /= currentIndex;

		}

		return currentAverageValueAdded;

	}

	public static float GetAverageProductivityEverywhere(string item) {

        if (!productivities.ContainsKey(item))
            Debug.LogError("Productivities does not contain " + item);
        float p = productivities[item];

		return p != 0 ? p : 1;

	}

	public static float GetAverageAutomationEverywhere(string item) {
		
		if (!productivities.ContainsKey(item))
			Debug.LogError("Automations does not contain " + item);
		float p = automationValue[item];

		return p;

	}

	public static void AddStructureToList(Structure str, string item) {

		generators[item].Add(str);

	}

	public static void RemoveStructureFromList(Structure str, string item) {

		generators[item].Remove(str);

	}

	public static void UpdateProductivities() {

		List<string> items = Enums.GetAllItems();
		foreach (string item in items) {
			productivities[item] = UpdateProductivity(item);
			automationValue[item] = UpdateAutomation(item);
			//Debug.Log(automationValue[item]);
		}

	}

    static float UpdateProductivity(string item) {

        float avgProductivity = GetAverageProductivityHere(item);  //begin with local productivity
        int numOfProducers = 1;
        
        //if we don't produce here, don't count it
        if (avgProductivity == 0)
            numOfProducers = 0;

        if (!competitors.ContainsKey(item))
            return avgProductivity != 0 ? avgProductivity : 1;

        List<City> producers = competitors[item];

        foreach (City c in producers) {
			
            numOfProducers++;
            avgProductivity = (avgProductivity * (numOfProducers - 1) + c.productivity) / numOfProducers;

        }

		//Debug.Log(item + " " + avgProductivity);

        return avgProductivity;

	}

	static float UpdateAutomation(string item) {

		float avgAutomation = GetAverageValueAddedHere(item);
		int numOfProducers = 1;

		//if we don't produce the item, don't count us as a producer
		if (GetAverageProductivityHere(item) == 0)
			numOfProducers = 0;

		//if no competitors, stop and return
		if (!competitors.ContainsKey(item))
			return avgAutomation;

		//FIGURE OUT HOW COMPETITORS HAVE TECHNOLOGY AND AUTOMATION

		return avgAutomation;

	}

}