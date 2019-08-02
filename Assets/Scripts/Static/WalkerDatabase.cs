using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class WalkerDatabase : MonoBehaviour {

	public TextAsset database;
	public static Dictionary<string, Dictionary<string, string>> walkerData = new Dictionary<string, Dictionary<string, string>>();
	
	public void Awake() {
		Enums.LoadDictionaries();
		ReadItemsFromDatabase();

	}

	public void ReadItemsFromDatabase() {
		XmlDocument doc = new XmlDocument();

		using (StringReader s = new StringReader(database.text)) {
			doc.Load(s);
		}

		XmlNodeList struList = doc.GetElementsByTagName("Walker");

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
				walkerData[name] = contents;

		}
	}
	
	public static WalkerData GetData(string s) {

		if (!walkerData.ContainsKey(s))
			return new WalkerData(s);

		//we have to return a new object so that way walkers aren't sharing data, which is important for returning home
		return new WalkerData(walkerData[s]);

	}

	public static bool HasData(string s) {

		return walkerData.ContainsKey(s);

	}

}