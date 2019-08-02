using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class City {

    public string name;
    public int distance = 6;
    public List<string> exports; //WHAT YOU CAN SELL TO THE CITY
    public List<string> imports;	//WHAT YOU CAN BUY FROM THE CITY
    public List<Technology> techs;
	public float productivity = 1;
	int techMax = 10;
    int espMax = 10;
    float techCount = 0;
    float espCount = 0;

	public City() { }

    public List<ItemOrder> GetPossibleExports() {

        List<ItemOrder> items = new List<ItemOrder>();

        foreach (string item in exports) 
            items.Add(new ItemOrder(100, item, this, TradeDirection.Export));

        return items;

    }

    public List<ItemOrder> GetPossibleImports() {

        List<ItemOrder> items = new List<ItemOrder>();

        foreach (string item in imports)
            items.Add(new ItemOrder(100, item, this, TradeDirection.Import));

        return items;

    }

    public override string ToString() {

        return name;

    }

    public bool RollTechnology(Technology tech) {

        int roll = Random.Range(1, 21);
        
        //UNSUCCESSFUL TECH RESEARCH
        if (roll > techCount) {

            //increase chance of research
            if (techCount < techMax)
                techCount += Difficulty.GetModifier();
            if (techCount > techMax)
                techCount = techMax;

            return false;

        }

        techCount = 0;
        techs.Add(tech);
        return true;

    }

    public bool RollEspionage() {

        int roll = Random.Range(1, 21);

        //UNSUCCESSFUL ESPIONAGE
        if (roll > espCount) {

            //increase chance of espionage
            if (espCount < espMax)
                espCount += Difficulty.GetModifier();
            if (espCount > espMax)
                techCount = espMax;

            return false;

        }

        espCount = 0;
        return true;

    }
	
}
