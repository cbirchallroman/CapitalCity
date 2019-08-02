using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class StructureDatabase : MonoBehaviour {

    public TextAsset database;
    public static Dictionary<string, StructureData> structureData = new Dictionary<string, StructureData>();

    public void Awake() {
        Enums.LoadDictionaries();
        ReadItemsFromDatabase();
    }

    void ReadItemsFromDatabase() {
        XmlDocument doc = new XmlDocument();

        using (StringReader s = new StringReader(database.text)) {
            doc.Load(s);
        }

        XmlNodeList struList = doc.GetElementsByTagName("Structure");

        foreach (XmlNode stru in struList) {

            XmlNodeList children = stru.ChildNodes;
            Dictionary<string, string> contents = new Dictionary<string, string>();
			string name = null;

			foreach (XmlNode thing in children) {
				if (thing.Name == "Name")
					name = thing.InnerText;
				contents.Add(thing.Name, thing.InnerText);
			}

			if (name != null)
				structureData[name] = new StructureData(contents);

		}
    }

    public static int GetModifiedCost(string s) {
        
        return Mathf.RoundToInt(GetBasePrice(s) * Difficulty.GetModifier());

    }

    public static int GetBasePrice(string s) {

        return structureData[s].baseCost;

    }

    public static StructureData GetData(string s) {

        if (!structureData.ContainsKey(s))
            return null;

        return structureData[s];

    }

    public static bool HasData(string s) {

        return structureData.ContainsKey(s);

    }

}
