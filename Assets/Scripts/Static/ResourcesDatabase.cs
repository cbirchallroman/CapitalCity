using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ResourcesDatabase : MonoBehaviour {

    public TextAsset database;
	public static float PoundsPerDays { get { return 50f / TimeController.DaysInAMonth / 100f; } }
		//50 is ideally how much we want to be contributed through each month of production
		//each production cycle lasts a month by default, depending on the item

    public static Dictionary<string, float> BasePrices = new Dictionary<string, float>();
    public static Dictionary<string, int> BaseDays = new Dictionary<string, int>();
    public static Dictionary<string, string[]> Ingredients = new Dictionary<string, string[]>();
	public static List<string> Whitelist;

    // Use this for initialization
    void Awake () {

        ReadItemsFromDatabase();
		
	}

	public static void CreateWhitelist() {

		Whitelist = new List<string>();

		//this is just a test whitelist but also useful to see what we have
		AddToWhitelist("Grain");
		AddToWhitelist("Pork");
		AddToWhitelist("Sand");
		AddToWhitelist("Wood");
		AddToWhitelist("Bricks");
		AddToWhitelist("Pottery");
		AddToWhitelist("Beer");
		AddToWhitelist("Cigars");
		AddToWhitelist("Beef");
		AddToWhitelist("Leather");

	}

	public static void AddToWhitelist(string item) {

		if(!Whitelist.Contains(item))
			Whitelist.Add(item);

		//add ingredients for item in case they were left off
		foreach (string ingredient in Ingredients[item]) {

			AddToWhitelist(ingredient.Split(' ')[1]);
		}

	}

	public static void LoadWhitelist(List<string> Whitelist) {



	}

	public static bool ItemAllowed(string item) {

		if (Whitelist.Count == 0)
			return true;	//if whitelist is empty, assume everything is okay
		return Whitelist.Contains(item);

	}

	public static string[] GetIngredients(string item) {

		return Ingredients[item];

	}

	public static int GetBaseDays(string item) {
        
		return BaseDays[item];

	}

	public static int GetAccumulativeDays(string item) {

		float days = GetBaseDays(item);

		//add the ingredients which go into the production of this item
		foreach (string s in Ingredients[item]) {

			string[] data = s.Split(' ');

			int amount2 = (int)(float.Parse(data[0]));      //amount / 100f needed to account for when the amount of what's being valued is not 100; preserve the ratio
			string item2 = data[1];

			if (string.IsNullOrEmpty(item2))
				Debug.LogError(item + " has a null ingredient");

			//if (item2.Equals(item))
			//	amount -= amount2;

			days += (float)GetAccumulativeDays(item2) * amount2 / 100f;

		}

		return (int)days;

	}

    public static float GetBasePrice(ItemOrder io) {

		return GetBasePrice(io.GetItemName(), io.amount);

    }

	public static float GetBasePrice(string item, int amount) {

		if (!BaseDays.ContainsKey(item))
			Debug.LogError("ResourceDatabase does not contain " + item);
		float baseDays = BaseDays[item];

        float globalProductivity = ProductivityController.GetAverageProductivityEverywhere(item);

        float price = baseDays * PoundsPerDays / globalProductivity;
		price = (float)Math.Round(price, 2);

		float ingredientsPrice = 0;

		//add the ingredients which go into the production of this item
		foreach (string s in Ingredients[item]) {

			string[] data = s.Split(' ');

			int amount2 = (int)(float.Parse(data[0]));		//amount / 100f needed to account for when the amount of what's being valued is not 100; preserve the ratio
			string item2 = data[1];

			if (string.IsNullOrEmpty(item2))
				Debug.LogError(item + " has a null ingredient");
			
			ingredientsPrice += GetBasePrice(item2, amount2);

		}

		//SOMEHOW DETERMINE WHAT CONSTANT CAPITAL (STEEL, WOOD, COAL, ETC) IS BEING INVESTED INTO THE PRODUCTION OF THE ITEM
		//	ADD THIS AVERAGE VALUE TO THE VALUE OF THE COMMODITY
		//	BC EACH BUILDING HAS A CONSTANT VALUE OF MACHINERY AND FUEL INVESTED INTO EACH UNIT OF PRODUCTION, THIS SHOULDN'T BE TOO HARD
		//	EXCEPT THAT WE HAVE TO SURVEY EACH BUILDING TO FIGURE OUT WHAT MACHINERY AND FUEL IS USED

		price += ingredientsPrice / 100f;
		price += ProductivityController.GetAverageAutomationEverywhere(item) / 100f;

		return price * amount;

	}

    void ReadItemsFromDatabase() {
		
        XmlDocument doc = new XmlDocument();

        using (StringReader s = new StringReader(database.text)) {
            doc.Load(s);
        }

        XmlNodeList struList = doc.GetElementsByTagName("Resource");

        foreach (XmlNode stru in struList) {

            XmlNodeList children = stru.ChildNodes;
            string item = null;
            int days = 16;
            float price = 1;
            List<string> ingredients = new List<string>();

            foreach (XmlNode thing in children) {
                if (thing.Name == "Name")
                    item = thing.InnerText;
                else if (thing.Name == "Price")
                    price = float.Parse(thing.InnerText);
                else if (thing.Name == "Ingredient")
                    ingredients.Add(thing.InnerText);
                else if (thing.Name == "Days")
                    days = int.Parse(thing.InnerText);
            }

            if (string.IsNullOrEmpty(item))
                Debug.LogError("Empty name for item in resources database");
            Ingredients[item] = ingredients.ToArray();
            BaseDays[item] = days;
            BasePrices[item] = price;
            

        }

    }
	
	public static Sprite GetSprite(string s) {

		Sprite spr = Resources.Load<Sprite>("Sprites/" + s);
		return spr;

	}

}
