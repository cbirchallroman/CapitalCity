using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum Terrain { Water, Lush, Grass, Mud, Sand, END }
public enum Month { March, April, May, June, July, August, September, October, November, December, January, February, END }
public enum Mode { Construct, Edit, Combat, END }
public enum DifficultyLevel { Easiest, Easy, Moderate, Hard, Hardest, END }
public enum LaborDivision { Materials, Production, Storage, Retail, Healthcare, Entertainment, Management, END }
public enum Season { Spring, Summer, Autumn, Winter, END }
public enum FoodType { Potatoes, Fish, Grain, Vegetables, Mutton, Pork, Beef, END }
public enum GoodType { Shoes, Trowsers, Pottery, Glassware, Carpet, Furniture, Coats, Beer, Cigars, WoodenToys, END }
public enum ResourceType { Bricks, Clay, Coal, Cotton, Denim, Flax, Leather, Linen, Ore, Paper, Sand, Spindles, Steel, Stone, Wood, Tobacco, Wool, Yarn, END }
public enum ItemType { Food, Good, Resource, END }
public enum Quality { None, Poor, Average, Great, Luxurious, END }
public enum NotificationType { Event, Issue, GoodNews, Invasion, END }
public enum TradeDirection { Export, Import, END }
public enum BuildingType { Administration, Culture, Distribution, Health, Industry, Resources, Agriculture, Crops, END }
public enum NaviType { Random, Leftward, Rightward, Straight, END }
public enum MachineType { Water, Steam, Stove, END }
public enum LaborType { Physical, Intellectual, Emotional, END }
public enum ImmigrantOrigin { American, Caribbean }
public enum Attitude { Miserable, Bored, Anxious, END }

public class Enums {
    public static Dictionary<string, Terrain> terrainDict = new Dictionary<string, Terrain>();
    public static Dictionary<string, Node> foodDict = new Dictionary<string, Node>();
    public static Dictionary<string, Node> resourceDict = new Dictionary<string, Node>();
    public static Dictionary<string, Node> goodDict = new Dictionary<string, Node>();
    public static Dictionary<string, Node> peopleDict = new Dictionary<string, Node>();
	public static Dictionary<string, BuildingType> categoryDict = new Dictionary<string, BuildingType>();
	public static Dictionary<string, MachineType> machineTypeDict = new Dictionary<string, MachineType>();

	public static void LoadDictionaries() {

        for (int x = 0; x < (int)Terrain.END; x++)
            terrainDict[(Terrain)x + ""] = (Terrain)x;

        //for food/goods, X is item, Y is type
        for (int x = 0; x < (int)FoodType.END; x++)
            foodDict[(FoodType)x + ""] = new Node(x, (int)ItemType.Food);

        for (int x = 0; x < (int)GoodType.END; x++)
            goodDict[(GoodType)x + ""] = new Node(x, (int)ItemType.Good);

        for (int x = 0; x < (int)ResourceType.END; x++)
            resourceDict[(ResourceType)x + ""] = new Node(x, (int)ItemType.Resource);

		for (int x = 0; x < (int)BuildingType.END; x++)
			categoryDict[(BuildingType)x + ""] = (BuildingType)x;

		for (int x = 0; x < (int)MachineType.END; x++)
			machineTypeDict[(MachineType)x + ""] = (MachineType)x;

	}

    public static string GetItemName(int index, ItemType type) {

		string name = "???";

        //if granary, display food label
        if (type == ItemType.Food)
			name = (FoodType)index + "";

        //if warehouse, display goods label
        else if (type == ItemType.Good)
			name = (GoodType)index + "";

        else if (type == ItemType.Resource)
			name = (ResourceType)index + "";
        
        return name;

    }

	public static string GetItemDisplayName(int index, ItemType type) {

		return Regex.Replace(GetItemName(index, type), "([a-z])([A-Z])", "$1 $2");

	}

	public static Node GetItemData(string item) {

		Node n = null;

		if (foodDict.ContainsKey(item))
			n = Enums.foodDict[item];

		else if (resourceDict.ContainsKey(item))
			n = Enums.resourceDict[item];

		else if (goodDict.ContainsKey(item))
			n = goodDict[item];

		else if (Enums.peopleDict.ContainsKey(item))
			n = peopleDict[item];

		else
			Debug.LogError(item + " does not exist as an item.");

		return n;

	}

	public static MachineType GetMachineType(string m) {

		return machineTypeDict.ContainsKey(m) ? machineTypeDict[m] : MachineType.END;

	}

	public static List<string> GetAllItems() {

		string[] allItems = new string[foodDict.Count + goodDict.Count + resourceDict.Count];
		foodDict.Keys.CopyTo(allItems, 0);
		goodDict.Keys.CopyTo(allItems, foodDict.Count);
		resourceDict.Keys.CopyTo(allItems, foodDict.Count + goodDict.Count);
		List<string> sortedItems = new List<string>(allItems);
		sortedItems.Sort();
		return sortedItems;

	}

}