using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class MachineDatabase : MonoBehaviour {

	public TextAsset database;
	public static Dictionary<string, Machine> machineData = new Dictionary<string, Machine>();

	public void Start() {
		Enums.LoadDictionaries();
		ReadItemsFromDatabase();
	}

	void ReadItemsFromDatabase() {
		XmlDocument doc = new XmlDocument();

		using (StringReader s = new StringReader(database.text)) {
			doc.Load(s);
		}

		XmlNodeList struList = doc.GetElementsByTagName("Machine");

		foreach (XmlNode stru in struList) {

			XmlNodeList children = stru.ChildNodes;
			Dictionary<string, string> contents = new Dictionary<string, string>();
			string type = null;

			foreach (XmlNode thing in children) {
				if (thing.Name == "Type")
					type = thing.InnerText;
				contents.Add(thing.Name, thing.InnerText);
			}

			if (type != null)
				machineData[type] = new Machine(contents);

		}
	}

	public static Machine GetMachineData(MachineType type) {

		string s = type + "";
		return machineData.ContainsKey(s) ? machineData[s] : null;

	}

}
